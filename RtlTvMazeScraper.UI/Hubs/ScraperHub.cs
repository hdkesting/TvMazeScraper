// <copyright file="ScraperHub.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.UI.Hubs
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.UI.ViewModels;
    using RtlTvMazeScraper.UI.Workers;

    /// <summary>
    /// Hub to report interact with the scraper.
    /// </summary>
    /// <seealso cref="Hub" />
    public class ScraperHub : Hub
    {
        private readonly IShowService showService;
        private readonly ITvMazeService tvMazeService;
        private readonly ILogger<ScraperHub> logger;
        private readonly ConsumeScopedScraperWorker scraperWorker;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScraperHub" /> class.
        /// </summary>
        /// <param name="showService">The show service.</param>
        /// <param name="tvMazeService">The tv maze service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="scraperWorker">The scraper worker.</param>
        public ScraperHub(
            IShowService showService,
            ITvMazeService tvMazeService,
            ILogger<ScraperHub> logger,
            ConsumeScopedScraperWorker scraperWorker)
        {
            this.showService = showService;
            this.tvMazeService = tvMazeService;
            this.logger = logger;
            this.scraperWorker = scraperWorker;
        }

        /// <summary>
        /// Sends the scrape result.
        /// </summary>
        /// <param name="show">The show that was just read.</param>
        /// <returns>A Task.</returns>
        public async Task SendScrapeResult(ScrapedShow show)
        {
            await this.Clients.All.SendAsync("ShowFound", show).ConfigureAwait(false);
        }

        /// <summary>
        /// Starts the scraping.
        /// </summary>
        /// <param name="startId">The start identifier.</param>
        /// <returns>A Task.</returns>
        public async Task StartScraping(int startId)
        {
            await this.StopScraping().ConfigureAwait(false);

            this.scraperWorker.ShowId = startId;
            await this.scraperWorker.StartAsync(default).ConfigureAwait(false);
        }

        /// <summary>
        /// Stops the scraping.
        /// </summary>
        /// <returns>A Task.</returns>
        public Task StopScraping()
        {
            return this.scraperWorker.StopAsync(default);
        }
    }
}
