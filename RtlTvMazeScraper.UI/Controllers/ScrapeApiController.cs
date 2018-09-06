// <copyright file="ScrapeApiController.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.UI.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.UI.Workers;

    /// <summary>
    /// An API controller to start/stop scraping.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/scraper")]
    public class ScrapeApiController : Controller
    {
        private readonly IShowService showService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrapeApiController" /> class.
        /// </summary>
        /// <param name="showService">The show service.</param>
        public ScrapeApiController(
            IShowService showService)
        {
            this.showService = showService;
        }

        /// <summary>
        /// Starts the scraping from the supplied <paramref name="showId" />.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <returns>A Task.</returns>
        [HttpGet("start")]
        public async Task Start(int showId)
        {
            StaticQueue.ClearQueue();
            StaticQueue.Enable();

            if (showId < 0)
            {
                showId = await this.showService.GetMaxShowId().ConfigureAwait(false);
                showId += 1;
            }

            StaticQueue.AddShowIds(showId);

            // assume the worker is continuously running and will pick this up shortly.
        }

        /// <summary>
        /// Stops the scraping.
        /// </summary>
        [HttpGet("stop")]
#pragma warning disable CA1822 // Mark members as static
        public void Stop()
#pragma warning restore CA1822 // Mark members as static
        {
            StaticQueue.ClearQueue();
        }
    }
}