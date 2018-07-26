// <copyright file="ScraperWorker.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.UI.Workers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.UI.Hubs;
    using RtlTvMazeScraper.UI.ViewModels;

    /// <summary>
    /// Scraper worker process.
    /// </summary>
    public sealed class ScraperWorker : BackgroundService
    {
        private readonly IShowService showService;
        private readonly ITvMazeService tvMazeService;
        private readonly ILogger<ScraperWorker> logger;
        private readonly IHubContext<ScraperHub> hubContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScraperWorker" /> class.
        /// </summary>
        /// <param name="showService">The show service.</param>
        /// <param name="tvMazeService">The tv maze service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="hubContext">The hub context.</param>
        public ScraperWorker(
            IShowService showService,
            ITvMazeService tvMazeService,
            ILogger<ScraperWorker> logger,
            IHubContext<ScraperHub> hubContext)
        {
            this.showService = showService;
            this.tvMazeService = tvMazeService;
            this.logger = logger;
            this.hubContext = hubContext;
        }

        /// <summary>
        /// Gets or sets the show identifier.
        /// </summary>
        /// <value>
        /// The show identifier.
        /// </value>
        public int ShowId { get; set; }

        /// <summary>
        /// This method is called when the <see cref="Microsoft.Extensions.Hosting.IHostedService" /> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" /> is called.</param>
        /// <returns>
        /// A <see cref="System.Threading.Tasks.Task" /> that represents the long running operations.
        /// </returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return this.PerformScraping(stoppingToken);
        }

        /// <summary>
        /// Performs the scraping.
        /// </summary>
        /// <returns>A Task.</returns>
        private async Task PerformScraping(CancellationToken stoppingToken)
        {
            int failedbatches = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                if (this.ShowId <= 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
                    continue;
                }

                var (show, status) = await this.tvMazeService.ScrapeSingleShowById(this.ShowId).ConfigureAwait(false);

                if (show != null)
                {
                    await this.showService.StoreShowList(new List<Core.Model.Show> { show }, null).ConfigureAwait(false);

                    var scraped = new ScrapedShow { Id = show.Id, Name = show.Name, CastCount = show.ShowCastMembers.Count };
                    await this.PostShow(scraped).ConfigureAwait(false);
                    await Task.Delay(100).ConfigureAwait(false);
                    failedbatches = 0;
                }
                else if (status == System.Net.HttpStatusCode.NotFound)
                {
                    failedbatches++;

                    if (failedbatches > 30)
                    {
                        // apparently no more shows to load
                        this.logger.LogInformation("Failed {failcount} times to get a show - aborting.", failedbatches);
                        break;
                    }

                    this.logger.LogInformation("Failed {failcount} times to get a show (and counting).", failedbatches);
                }

                if (status != Core.Support.Constants.ServerTooBusy)
                {
                    this.ShowId = this.ShowId + 1;
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
                }
            }

            this.ShowId = 0;
        }

        private Task PostShow(ScrapedShow show)
        {
            return this.hubContext.Clients.All.SendAsync("ShowFound", show);
        }
    }
}
