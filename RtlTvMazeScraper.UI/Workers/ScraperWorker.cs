// <copyright file="ScraperWorker.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.UI.Workers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;
    using TvMazeScraper.Core.Interfaces;
    using TvMazeScraper.UI.Hubs;
    using TvMazeScraper.UI.ViewModels;

    /// <summary>
    /// Scraper worker process.
    /// </summary>
    public sealed class ScraperWorker : IScraperWorker
    {
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
        /// Does the work on a single show.
        /// </summary>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task<WorkResult> DoWorkOnSingleShow()
        {
            try
            {
                return await this.PerformScraping(default).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(ex, "DoWorkOnSingleShow failed");
                return WorkResult.Error;
            }
        }

        /// <summary>
        /// Does the work by reading many shows.
        /// </summary>
        /// <returns>A Task.</returns>
        public async Task<WorkResult> DoWorkOnManyShows()
        {
            WorkResult res;

            try
            {
                while ((res = await this.PerformScraping(default).ConfigureAwait(false)) == WorkResult.Done)
                {
                    this.logger.LogTrace("Got another one");
                }

                return res;
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(ex, "DoWorkOnManyShows failed");
                return WorkResult.Error;
            }
        }

        /// <summary>
        /// Performs the scraping of a single show.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        private async Task<WorkResult> PerformScraping(CancellationToken stoppingToken)
        {
            try
            {
                var showId = StaticQueue.GetNextId();

                if (!showId.HasValue)
                {
                    return WorkResult.Empty;
                }

                var (show, status) = await this.tvMazeService.ScrapeSingleShowById(showId.Value, stoppingToken).ConfigureAwait(false);

                if (!(show is null))
                {
                    await this.showService.StoreShowList(new List<Core.DTO.ShowDto> { show }, null).ConfigureAwait(false);

                    var scraped = new ScrapedShow { Id = show.Id, Name = show.Name, CastCount = show.CastMembers.Count };
                    await this.PostShow(scraped).ConfigureAwait(false);

                    // make sure more shows are queued, now that one has been processed. Note that this will fail if there is a gap >30
                    StaticQueue.AddShowIds(showId.Value + 1, 30);
                }

                //// no need to handle "404" as that just depletes the queue which automatically halts checking

                if (status == Core.Support.Constants.ServerTooBusy)
                {
                    // I don't expect this to happen (or rather: "already handled")
                    StaticQueue.AddShowIds(showId.Value, 1); // retry later
                    return WorkResult.Busy;
                }

                return WorkResult.Done;
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(ex, "PerformScraping failed.");
                return WorkResult.Error;
            }
        }

        private Task PostShow(ScrapedShow show)
        {
            return this.hubContext.Clients.All.SendAsync("ShowFound", show);
        }
    }
}
