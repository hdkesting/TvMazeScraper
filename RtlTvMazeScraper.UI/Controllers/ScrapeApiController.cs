// <copyright file="ScrapeApiController.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.UI.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using RtlTvMazeScraper.UI.Workers;

    /// <summary>
    /// An API controller to start/stop scraping.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/scraper")]
    public class ScrapeApiController : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScrapeApiController"/> class.
        /// </summary>
        public ScrapeApiController()
        {
        }

        /// <summary>
        /// Starts the scraping from the supplied <paramref name="showId"/>.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        [HttpGet("start")]
        public void Start(int showId)
        {
            StaticQueue.ClearQueue();
            StaticQueue.AddShowIds(showId);

            // assume the worker is continuously running and will pick this up shortly.
        }

        /// <summary>
        /// Stops the scraping.
        /// </summary>
        [HttpGet("stop")]
        public void Stop()
        {
            StaticQueue.ClearQueue();
        }
    }
}