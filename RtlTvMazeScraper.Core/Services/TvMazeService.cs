// <copyright file="TvMazeService.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Model;
    using RtlTvMazeScraper.Core.Transfer;

    /// <summary>
    /// The service that reads TV Maze.
    /// </summary>
    /// <seealso cref="ITvMazeService" />
    public class TvMazeService : ITvMazeService
    {
        private const int DefaultMaximumShows = 40;

        private readonly string hostname;
        private readonly IApiRepository apiRepository;
        private readonly ILogRepository logRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TvMazeService" /> class.
        /// </summary>
        /// <param name="settingRepository">The setting repository.</param>
        /// <param name="apiRepository">The API repository.</param>
        /// <param name="logRepository">The log repository.</param>
        public TvMazeService(
            ISettingRepository settingRepository,
            IApiRepository apiRepository,
            ILogRepository logRepository)
        {
            this.hostname = settingRepository.TvMazeHost;
            this.apiRepository = apiRepository;
            this.logRepository = logRepository;
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
        /// <returns>
        /// A list of shows.
        /// </returns>
        public async Task<List<Show>> ScrapeShowsBySearch(string searchWord)
        {
            var (status, json) = await this.apiRepository.RequestJson($"{this.hostname}/search/shows?q={searchWord}", retryOnBusy: true);

            if (status != HttpStatusCode.OK)
            {
                // some error or 429
                this.logRepository.Log(Support.LogLevel.Warning, $"Scraping shows for '{searchWord}' returned status {status}.");
                return null;
            }

            var result = new List<Show>();

            // read json
            var array = JArray.Parse(json);
            foreach (var showcontainer in array)
            {
                var jshow = (JObject)showcontainer["show"];
                var show = new Show()
                {
                    Id = (int)jshow["id"],
                    Name = (string)jshow["name"],
                };

                result.Add(show);
            }

            this.logRepository.Log(Support.LogLevel.Information, $"Scraping for '{searchWord}' returned {result.Count} shows.");
            return result;
        }

        /// <summary>
        /// Scrapes the cast members for a particular show.
        /// </summary>
        /// <param name="showid">The showid.</param>
        /// <returns>
        /// A list of cast members.
        /// </returns>
        public async Task<List<CastMember>> ScrapeCastMembers(int showid)
        {
            var (status, json) = await this.apiRepository.RequestJson($"{this.hostname}/shows/{showid}/cast", retryOnBusy: true);

            if (status != HttpStatusCode.OK)
            {
                this.logRepository.Log(Support.LogLevel.Warning, $"Scraping cast for #{showid} returned status {status}.");
                return null;
            }

            var result = new List<CastMember>();

            // read json
            var array = JArray.Parse(json);
            foreach (var role in array)
            {
                var person = (JObject)role["person"];
                var member = new CastMember()
                {
                    MemberId = (int)person["id"],
                    ShowId = showid,
                    Name = (string)person["name"],
                };

                var bd = person["birthday"];
                member.Birthdate = this.GetDate(bd);

                result.Add(member);
            }

            this.logRepository.Log(Support.LogLevel.Information, $"Found {result.Count} cast members for show #{showid}.");
            return result;
        }

        /// <summary>
        /// Scrapes shows by their identifier.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns>
        /// A tuple: number of shows tried, list of shows found.
        /// </returns>
        public async Task<ScrapeBatchResult> ScrapeById(int start)
        {
            int count = 0;

            var list = new List<Show>();

            while (count < this.MaxNumberOfShowsToScrape)
            {
                var (status, json) = await this.apiRepository.RequestJson($"{this.hostname}/shows/{start + count}?embed=cast", false);

                if (status == Support.Constants.ServerTooBusy)
                {
                    // too much, so back off
                    this.logRepository.Log(Support.LogLevel.Debug, $"Server too busy to scrape #{start + count}, backing off.");
                    break;
                }

                if (status == HttpStatusCode.OK)
                {
                    var jshow = JObject.Parse(json);
                    var show = new Show
                    {
                        Id = (int)jshow["id"],
                        Name = (string)jshow["name"],
                    };

                    var jcast = (JArray)jshow["_embedded"]["cast"];
                    foreach (var container in jcast)
                    {
                        var person = (JObject)container["person"];
                        var member = new CastMember
                        {
                            MemberId = (int)person["id"],
                            ShowId = show.Id,
                            Name = (string)person["name"],
                        };

                        var bd = person["birthday"];
                        member.Birthdate = this.GetDate(bd);

                        show.CastMembers.Add(member);
                    }

                    list.Add(show);
                }

                count++;
            }

            this.logRepository.Log(Support.LogLevel.Information, $"Scraping shows from {start} returned {list.Count} results out of {count} tried.");
            return new ScrapeBatchResult(count, list);
        }

        private DateTime? GetDate(JToken dayValue)
        {
            if (dayValue.Type == JTokenType.Date)
            {
                return (DateTime?)dayValue;
            }
            else if (dayValue.Type == JTokenType.String)
            {
                if (DateTime.TryParseExact(dayValue.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                {
                    return dt.Date;
                }
            }

            return null;
        }
    }
}