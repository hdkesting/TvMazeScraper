﻿// <copyright file="TvMazeService.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Core.Services
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
    using TvMazeScraper.Core.DTO;
    using TvMazeScraper.Core.Interfaces;
    using TvMazeScraper.Core.Support;
    using TvMazeScraper.Core.Transfer;

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
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        public async Task<List<ShowDto>> ScrapeShowsBySearch(string searchWord, CancellationToken cancellationToken = default)
        {
            // note: not documented, but apparently returns just the first 10 results
            var (status, json) = await this.apiRepository.RequestJsonForTvMaze(
                    new Uri($"/search/shows?q={searchWord}", UriKind.Relative),
                    cancellationToken)
                .ConfigureAwait(false);

            if (status != HttpStatusCode.OK)
            {
                // some error or 429
                this.logger.LogWarning("Scraping shows for '{searchWord}' returned status {status}.", searchWord, status);
                return null;
            }

            var result = new List<ShowDto>();

            /* NOTE to reviewer: I could have created a tree of objects to deserialize this JSON into,
             * but that would be a tree per API call with no benefit to speed of execution, speed of development
             * or maintainability. A change in the API would in both cases force a rewrite.
            */

            // read json
            var shows = JArray.Parse(json);

            foreach (dynamic showcontainer in shows)
            {
                var jshow = showcontainer.show;
                var show = new ShowDto
                {
                    Id = jshow.id,
                    Name = jshow.name,
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
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A list of cast members.
        /// </returns>
        public async Task<List<CastMemberDto>> ScrapeCastMembers(int showid, CancellationToken cancellationToken = default)
        {
            var (status, json) = await this.apiRepository.RequestJsonForTvMaze(
                    new Uri($"/shows/{showid}/cast", UriKind.Relative),
                    cancellationToken)
                .ConfigureAwait(false);

            if (status != HttpStatusCode.OK)
            {
                this.logger.LogWarning("Scraping cast for #{showid} returned status {status}.", showid, status);
                return null;
            }

            var result = new List<CastMemberDto>();

            // read json
            var roles = JArray.Parse(json);

            foreach (dynamic role in roles)
            {
                var person = role.person;
                var member = new CastMemberDto
                {
                    Id = person.id,
                    Name = person.name,
                    Birthdate = person.birthday,
                };

                result.Add(member);
            }

            this.logger.LogInformation("Found {resultCount} cast members for show #{showid}.", result.Count, showid);
            return result;
        }

        /// <summary>
        /// Scrapes a batch of shows by their identifier, starting from the supplied <paramref name="start" />.
        /// </summary>
        /// <param name="start">The start ID.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A tuple: number of shows tried, list of shows found.
        /// </returns>
        public async Task<ScrapeBatchResult> ScrapeBatchById(int start, CancellationToken cancellationToken = default)
        {
            int count = 0;

            var list = new List<ShowDto>();

            var backoff = false;

            while (count < this.MaxNumberOfShowsToScrape && !backoff)
            {
                int currentId = start + count;

                var (show, status) = await this.ScrapeSingleShowById(currentId, cancellationToken).ConfigureAwait(false);

                if (status == HttpStatusCode.OK)
                {
                    list.Add(show);
                }
                else if (status == Constants.ServerTooBusy)
                {
                    backoff = true;
                }

                /* just ignore a "not found" response. */

                count++;
            }

            this.logger.LogInformation("Scraping shows from {start} returned {returnedCount} results out of {requestedCount} tried.", start, list.Count, count);
            return new ScrapeBatchResult(count, list);
        }

        /// <summary>
        /// Scrapes the single show by its identifier.
        /// </summary>
        /// <param name="showId">The show's identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task with Show and status code.</returns>
        public async Task<ScrapeResult> ScrapeSingleShowById(int showId, CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            var (status, json) = await this.apiRepository.RequestJsonForTvMaze(
                    new Uri($"/shows/{showId}?embed=cast", UriKind.Relative),
                    cancellationToken)
                .ConfigureAwait(false);
            sw.Stop();
            this.logger.LogInformation("Getting info about show {ShowId} took {Elapsed} msec and returned status {HttpStatus}.", showId, sw.ElapsedMilliseconds, status);

            if (status == Constants.ServerTooBusy)
            {
                // too much, so back off
                this.logger.LogDebug("Server too busy to scrape #{ShowId}, backing off.", showId);
                return new ScrapeResult { HttpStatus = status };
            }

            if (status == HttpStatusCode.OK)
            {
                dynamic jshow = JObject.Parse(json);
                var show = new ShowDto
                {
                    Id = jshow.id,
                    Name = jshow.name,
                    ImdbId = jshow.externals?.imdb,
                };

                var embedded = jshow._embedded;
                if (embedded is null)
                {
                    this.logger.LogError("Server didn't return the requested embedded data for show {ShowId}.", showId);
                }
                else
                {
                    var jcast = embedded.cast;
                    if (jcast is null)
                    {
                        this.logger.LogError("Server didn't return the requested cast in the embedded data for show {ShowId}.", showId);
                    }
                    else
                    {
                        foreach (var container in jcast)
                        {
                            var person = container.person;
                            var member = new CastMemberDto
                            {
                                Id = person.id,
                                Name = person.name,
                                Birthdate = person.birthday,
                            };

                            show.CastMembers.Add(member);
                        }
                    }
                }

                return new ScrapeResult { Show = show, HttpStatus = status };
            }

            return new ScrapeResult { HttpStatus = status };
        }
    }
}