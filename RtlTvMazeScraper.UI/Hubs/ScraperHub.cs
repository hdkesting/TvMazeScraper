// <copyright file="ScraperHub.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.UI.Hubs
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;
    using TvMazeScraper.UI.ViewModels;

    /// <summary>
    /// Hub to report interact with the scraper.
    /// </summary>
    /// <seealso cref="Hub" />
    public class ScraperHub : Hub
    {
        private readonly ILogger<ScraperHub> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScraperHub" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ScraperHub(
            ILogger<ScraperHub> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Sends the scrape result.
        /// </summary>
        /// <param name="show">The show that was just read.</param>
        /// <returns>A Task.</returns>
        public Task SendScrapeResult(ScrapedShow show)
        {
            return this.Clients.All.SendAsync("ShowFound", show);
        }
    }
}
