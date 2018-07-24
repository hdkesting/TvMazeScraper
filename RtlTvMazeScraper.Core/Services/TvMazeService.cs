// <copyright file="TvMazeService.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Model;
    using RtlTvMazeScraper.Core.Transfer;

    /// <summary>
    /// The service that reads TV Maze.
    /// </summary>
    /// <seealso cref="ITvMazeService" />
    [CLSCompliant(false)]
    public class TvMazeService : ITvMazeService
    {
        private const int DefaultMaximumShows = 20;

        private readonly IApiRepository apiRepository;
        private readonly ILogger<TvMazeService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TvMazeService" /> class.
        /// </summary>
        /// <param name="apiRepository">The API repository.</param>
        /// <param name="logger">The logger.</param>
        public TvMazeService(
            IApiRepository apiRepository,
            ILogger<TvMazeService> logger)
        {
            this.apiRepository = apiRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Gets or sets the maximum number of shows to scrape per invocation.
        /// </summary>
        /// <remarks>
        /// Mostly useful in unittesting.
        /// </remarks>
        /// <value>
        /// The maximum number of shows to scrape.
        /// </value>
        public int MaxNumberOfShowsToScrape { get; set; } = DefaultMaximumShows;

        /// <summary>
        /// Scrapes the shows by their initial.
        /// </summary>
        /// <param name="searchWord">The search word.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        public async Task<List<Show>> ScrapeShowsBySearch(string searchWord, CancellationToken cancellationToken = default)
        {
            // note: not documented, but apparently returns just the first 10 results
            var (status, json) = await this.apiRepository.RequestJson(
                    $"/search/shows?q={searchWord}",
                    cancellationToken)
                .ConfigureAwait(false);

            if (status != HttpStatusCode.OK)
            {
                // some error or 429
                this.logger.LogWarning("Scraping shows for '{searchWord}' returned status {status}.", searchWord, status);
                return null;
            }

            var result = new List<Show>();

            /* NOTE to reviewer: I could have created a tree of objects to deserialize this JSON into,
             * but that would be a tree per API call with no benefit to speed of execution, speed of development
             * or maintainability. A change in the API would in both cases force a rewrite.
            */

            // read json
            var array = JArray.Parse(json);
            foreach (var showcontainer in array)
            {
                var jshow = (JObject)showcontainer[Support.TvMazeSearchResultNames.ShowContainer];
                var show = new Show()
                {
                    Id = (int)jshow[Support.TvMazeSearchResultNames.ShowId],
                    Name = (string)jshow[Support.TvMazeSearchResultNames.ShowName],
                };

                result.Add(show);
            }

            this.logger.LogInformation("Scraping for '{searchWord}' returned {resultCount} shows.", searchWord, result.Count);
            return result;
        }

        /// <summary>
        /// Scrapes the cast members for a particular show.
        /// </summary>
        /// <param name="showid">The showid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A list of cast members.
        /// </returns>
        public async Task<List<CastMember>> ScrapeCastMembers(int showid, CancellationToken cancellationToken = default)
        {
            var (status, json) = await this.apiRepository.RequestJson(
                    $"/shows/{showid}/cast",
                    cancellationToken)
                .ConfigureAwait(false);

            if (status != HttpStatusCode.OK)
            {
                this.logger.LogWarning("Scraping cast for #{showid} returned status {status}.", showid, status);
                return null;
            }

            var result = new List<CastMember>();

            // read json
            var array = JArray.Parse(json);
            foreach (var role in array)
            {
                var person = (JObject)role[Support.TvMazeCastResultNames.PersonContainer];
                var member = new CastMember()
                {
                    Id = (int)person[Support.TvMazeCastResultNames.PersonId],
                    Name = (string)person[Support.TvMazeCastResultNames.PersonName],
                };

                var bd = person[Support.TvMazeCastResultNames.PersonBirthday];
                member.Birthdate = GetDate(bd);

                result.Add(member);
            }

            this.logger.LogInformation("Found {resultCount} cast members for show #{showid}.", result.Count, showid);
            return result;
        }

        /// <summary>
        /// Scrapes shows by their identifier.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A tuple: number of shows tried, list of shows found.
        /// </returns>
        public async Task<ScrapeBatchResult> ScrapeById(int start, CancellationToken cancellationToken = default)
        {
            int count = 0;

            var list = new List<Show>();

            while (count < this.MaxNumberOfShowsToScrape)
            {
                int currentId = start + count;
                var sw = Stopwatch.StartNew();
                var (status, json) = await this.apiRepository.RequestJson(
                        $"/shows/{currentId}?embed=cast",
                        cancellationToken)
                    .ConfigureAwait(false);
                sw.Stop();
                this.logger.LogInformation("Getting info about show {ShowId} took {Elapsed} msec and returned status {HttpStatus}.", currentId, sw.ElapsedMilliseconds, status);

                if (status == Support.Constants.ServerTooBusy)
                {
                    // too much, so back off
                    this.logger.LogDebug("Server too busy to scrape #{ShowId}, backing off.", currentId);
                    break;
                }

                if (status == HttpStatusCode.OK)
                {
                    var jshow = JObject.Parse(json);
                    var show = new Show
                    {
                        Id = (int)jshow[Support.TvMazeShowWithCastResultNames.ShowId],
                        Name = (string)jshow[Support.TvMazeShowWithCastResultNames.ShowName],
                    };

                    var embedded = jshow[Support.TvMazeShowWithCastResultNames.EmbeddedContainer];
                    if (embedded == null)
                    {
                        this.logger.LogError("Server didn't return the requested embedded data for show {ShowId}.", currentId);
                    }
                    else
                    {
                        var jcast = (JArray)embedded[Support.TvMazeShowWithCastResultNames.CastContainer];
                        if (jcast == null)
                        {
                            this.logger.LogError("Server didn't return the requested cast in the embedded data for show {ShowId}.", currentId);
                        }
                        else
                        {
                            foreach (var container in jcast)
                            {
                                var person = (JObject)container[Support.TvMazeShowWithCastResultNames.PersonContainer];
                                var member = new CastMember
                                {
                                    Id = (int)person[Support.TvMazeShowWithCastResultNames.PersonId],
                                    Name = (string)person[Support.TvMazeShowWithCastResultNames.PersonName],
                                };

                                var bd = person[Support.TvMazeShowWithCastResultNames.PersonBirthday];
                                member.Birthdate = GetDate(bd);

                                show.ShowCastMembers.Add(new ShowCastMember
                                {
                                    Show = show,
                                    ShowId = show.Id,
                                    CastMemberId = member.Id,
                                    CastMember = member,
                                });
                            }
                        }
                    }

                    list.Add(show);
                }

                /* just ignore a "not found" response. */

                count++;
            }

            this.logger.LogInformation("Scraping shows from {start} returned {returnedCount} results out of {requestedCount} tried.", start, list.Count, count);
            return new ScrapeBatchResult(count, list);
        }

        private static DateTime? GetDate(JToken dayValue)
        {
            const string expectedDateFormat = "yyyy-MM-dd";

            if (dayValue.Type == JTokenType.Date)
            {
                return (DateTime?)dayValue;
            }
            else if (dayValue.Type == JTokenType.String)
            {
                if (DateTime.TryParseExact(dayValue.ToString(), expectedDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                {
                    return dt;
                }
            }

            return null;
        }
    }
}