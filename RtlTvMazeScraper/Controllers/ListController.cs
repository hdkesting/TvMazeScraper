// <copyright file="ListController.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Newtonsoft.Json.Linq;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Support;

    /// <summary>
    /// A (WebAPI) controller to show lists of shows.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class ListController : ApiController
    {
        private readonly IShowService showService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListController" /> class.
        /// </summary>
        /// <param name="showService">The show service.</param>
        public ListController(IShowService showService)
        {
            this.showService = showService;
        }

        /// <summary>
        /// Gets one page of the shows as <see cref="JArray"/> (which will be sent as JSON to the browser).
        /// </summary>
        /// <remarks>
        /// This method build a JSON string "by hand".
        /// </remarks>
        /// <param name="page">The page number (0-based).</param>
        /// <param name="pagesize">The size of the page.</param>
        /// <returns>A JSON result.</returns>
        [HttpGet]
        public async Task<JArray> GetShows(int page = 0, int pagesize = 20)
        {
            if (page < 0)
            {
                page = 0;
            }

            if (pagesize < 2)
            {
                pagesize = 2;
            }

            var shows = await this.showService.GetShowsWithCast(page, pagesize);

            JArray result = new JArray();

            foreach (var show in shows)
            {
                var cast = new JArray();

                // order cast by birthdate, descending. As per requirement.
                foreach (var member in show.CastMembers.OrderByDescending(m => m.Birthdate))
                {
                    var cm = new JObject(
                        new JProperty("id", member.MemberId),
                        new JProperty("name", member.Name),
                        new JProperty("birthday", member.Birthdate.HasValue ? member.Birthdate.Value.ToString("yyyy-MM-dd") : null));
                    cast.Add(cm);
                }

                var showObj = new JObject(
                    new JProperty("id", show.Id),
                    new JProperty("name", show.Name),
                    new JProperty("cast", cast));

                result.Add(showObj);
            }

            return result;
        }

        /// <summary>
        /// Gets the shows as JSON-serializable objects.
        /// </summary>
        /// <remarks>
        /// Note that some configuration is done in WebApiConfig.cs.
        /// The list is sent as JSON to the browser, because XML serialization is switched off.
        /// </remarks>
        /// <param name="page">The page.</param>
        /// <param name="pagesize">The pagesize.</param>
        /// <returns>A JSON-serializable list.</returns>
        [HttpGet]
        [Route("~/list")]
        public async Task<List<ShowForJson>> GetShows2(int page = 0, int pagesize = 20)
        {
            if (page < 0)
            {
                page = 0;
            }

            if (pagesize < 2)
            {
                pagesize = 2;
            }

            var dbshows = await this.showService.GetShowsWithCast(page, pagesize);

            var result = Support.Converter.Convert(dbshows);
            return result;
        }
    }
}
