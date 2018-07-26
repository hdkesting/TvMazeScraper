// <copyright file="ScraperWorker.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.UI.Workers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.UI.Hubs;
    using RtlTvMazeScraper.UI.ViewModels;

    /// <summary>
    /// Scraper worker process.
    /// </summary>
    public sealed class ScraperWorker : IScraperWorker
    {
        private static readonly Queue<int> ShowIdQueue = new Queue<int>();

        private readonly IShowService showService;
        private readonly ITvMazeService tvMazeService;
        private readonly IHubContext<ScraperHub> hubContext;
        private readonly ILogger<ScraperWorker> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScraperWorker" /> class.
        /// </summary>
        /// <param name="showService">The show service.</param>
        /// <param name="tvMazeService">The tv maze service.</param>
        /// <param name="hubContext">The hub context.</param>
        /// <param name="logger">The logger.</param>
        public ScraperWorker(
            IShowService showService,
            ITvMazeService tvMazeService,
            IHubContext<ScraperHub> hubContext,
            ILogger<ScraperWorker> logger)
        {
            this.showService = showService;
            this.tvMazeService = tvMazeService;
            this.hubContext = hubContext;
            this.logger = logger;
        }

        /// <summary>
        /// Adds the show ids to the queue.
        /// </summary>
        /// <param name="startId">The start identifier.</param>
        /// <param name="count">The count.</param>
        public void AddShowIds(int startId, int count = 10)
        {
            for (int n = 0; n < count; n++)
            {
                int id = startId + n;
                if (!ShowIdQueue.Contains(id))
                {
                    ShowIdQueue.Enqueue(id);
                }
            }
        }

        /// <summary>
        /// Clears the queue.
        /// </summary>
        public void ClearQueue()
        {
            ShowIdQueue.Clear();
        }

        /// <summary>
        /// Does the work.
        /// </summary>
        public void DoWork()
        {
            try
            {
                this.PerformScraping(default).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(ex, "ExecuteAsync failed");
            }
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
                if (ShowIdQueue.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                    continue;
                }

                int showId = ShowIdQueue.Dequeue();

                var (show, status) = await this.tvMazeService.ScrapeSingleShowById(showId).ConfigureAwait(false);

                if (show != null)
                {
                    await this.showService.StoreShowList(new List<Core.Model.Show> { show }, null).ConfigureAwait(false);

                    var scraped = new ScrapedShow { Id = show.Id, Name = show.Name, CastCount = show.ShowCastMembers.Count };
                    await this.PostShow(scraped).ConfigureAwait(false);
                    await Task.Delay(100).ConfigureAwait(false);
                    this.AddShowIds(showId);
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

                if (status == Core.Support.Constants.ServerTooBusy)
                {
                    this.AddShowIds(showId, 1); // retry later
                    await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
                }
            }
        }

        private Task PostShow(ScrapedShow show)
        {
            return this.hubContext.Clients.All.SendAsync("ShowFound", show);
        }
    }
}
