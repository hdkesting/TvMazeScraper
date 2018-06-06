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
    using RtlTvMazeScraper.Core.Models;

    /// <summary>
    /// The service that reads TV Maze.
    /// </summary>
    /// <seealso cref="RtlTvMazeScraper.Core.Interfaces.ITvMazeService" />
    /// <seealso cref="RtlTvMazeScraper.Interfaces.ITvMazeService" />
    public class TvMazeService : ITvMazeService
    {
        private const int DefaultMaximumShows = 40;

        private readonly string hostname;
        private readonly IApiRepository apiRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TvMazeService" /> class.
        /// </summary>
        /// <param name="settingRepository">The setting repository.</param>
        /// <param name="apiRepository">The API repository.</param>
        public TvMazeService(
            ISettingRepository settingRepository,
            IApiRepository apiRepository)
        {
            this.hostname = settingRepository.TvMazeHost;
            this.apiRepository = apiRepository;
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
                    Id = (int)person["id"],
                    Name = (string)person["name"],
                };

                var bd = person["birthday"];
                member.Birthdate = this.GetDate(bd);

                result.Add(member);
            }

            return result;
        }

        /// <summary>
        /// Scrapes shows by their identifier.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns>
        /// A tuple: number of shows tried, list of shows found.
        /// </returns>
        public async Task<(int count, List<Show> shows)> ScrapeById(int start)
        {
            int count = 0;

            var list = new List<Show>();

            while (count < this.MaxNumberOfShowsToScrape)
            {
                var (status, json) = await this.apiRepository.RequestJson($"{this.hostname}/shows/{start + count}?embed=cast", false);

                if (status == (HttpStatusCode)429)
                {
                    // too much, so back off
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
                            Id = (int)person["id"],
                            Name = (string)person["name"],
                        };

                        var bd = person["birthday"];
                        member.Birthdate = this.GetDate(bd);

                        show.Cast.Add(member);
                    }

                    list.Add(show);
                }

                count++;
            }

            return (count, list);
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