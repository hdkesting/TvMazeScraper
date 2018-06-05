// <copyright file="ListController.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using Newtonsoft.Json.Linq;
    using RtlTvMazeScraper.Core.Interfaces;

    /// <summary>
    /// A (WebAPI) controller to show lists of shows.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class ListController : ApiController
    {
        private readonly IShowRepository showRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListController"/> class.
        /// </summary>
        /// <param name="showRepository">The show repository.</param>
        public ListController(IShowRepository showRepository)
        {
            this.showRepository = showRepository;
        }

        /// <summary>
        /// Gets one page of the shows.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pagesize">The pagesize.</param>
        /// <returns>A JSON result.</returns>
        [HttpGet]
        [Route("~/list")]
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

            var shows = await this.showRepository.GetShowsWithCast(page, pagesize);

            JArray result = new JArray();

            foreach (var show in shows)
            {
                var cast = new JArray();
                foreach (var member in show.Cast)
                {
                    var cm = new JObject(
                        new JProperty("id", member.Id),
                        new JProperty("name", member.Name),
                        new JProperty("birthday", member.Birthdate));
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
    }
}
