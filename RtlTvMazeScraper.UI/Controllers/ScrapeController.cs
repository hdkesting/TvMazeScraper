﻿// <copyright file="ScrapeController.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.UI.Controllers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TvMazeScraper.Core.Interfaces;
    using TvMazeScraper.UI.ViewModels;

    /// <summary>
    /// A controller that performs the scraping of the TvMaze site.
    /// </summary>
    public class ScrapeController : Controller
    {
        private readonly IShowService showService;
        private readonly ITvMazeService tvMazeService;
        private readonly ILogger<ScrapeController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrapeController" /> class.
        /// </summary>
        /// <param name="showService">The show service.</param>
        /// <param name="tvMazeService">The tv maze service.</param>
        /// <param name="logger">The log service.</param>
        public ScrapeController(
            IShowService showService,
            ITvMazeService tvMazeService,
            ILogger<ScrapeController> logger)
        {
            this.showService = showService;
            this.tvMazeService = tvMazeService;
            this.logger = logger;
        }

        /// <summary>
        /// Shows the default page.
        /// </summary>
        /// <returns>A View.</returns>
        public async Task<ActionResult> Index()
        {
            var max = await this.showService.GetMaxShowId().ConfigureAwait(false);

            return this.View(max);
        }

        /// <summary>
        /// Scrapes by searching for an initial.
        /// </summary>
        /// <param name="initial">The initial to search for.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A View or a redirect.
        /// </returns>
        public async Task<ActionResult> ScrapeAlpha(string initial = null, CancellationToken cancellationToken = default)
        {
            const string firstInitial = "A";
            const string lastInitial = "Z";

            var model = new ScrapeAlphaViewModel();

            if (string.IsNullOrEmpty(initial))
            {
                initial = firstInitial;
            }
            else
            {
                initial = initial.Substring(0, 1).ToUpperInvariant();
                if (string.Compare(initial, firstInitial, StringComparison.Ordinal) < 0)
                {
                    initial = firstInitial;
                }
                else if (string.Compare(initial, lastInitial, StringComparison.Ordinal) > 0)
                {
                    initial = lastInitial;
                }

                model.PreviousInitial = initial;

                // perform scrape
                var list = await this.tvMazeService.ScrapeShowsBySearch(initial, cancellationToken).ConfigureAwait(false);

                if (list is null)
                {
                    this.logger.LogWarning("Scraping for {Initial} returned no results.", initial);
                    model.PreviousCount = 0;
                }
                else
                {
                    this.logger.LogInformation("Scraping for {Initial} returned {Count} results.", initial, list.Count);
                    model.PreviousCount = list.Count;

                    await this.showService.StoreShowList(list, id => this.tvMazeService.ScrapeCastMembers(id)).ConfigureAwait(false);

                    if (initial.StartsWith(lastInitial, StringComparison.Ordinal))
                    {
                        // done!
                        return this.RedirectToAction(nameof(this.Index));
                    }
                }

                // setup for next initial
                initial = ((char)(initial[0] + 1)).ToString(CultureInfo.InvariantCulture);
            }

            model.NextInitial = initial;

            return this.View(model);
        }

        /// <summary>
        /// Scrapes one batch from the specified start ID (defaults to 1).
        /// </summary>
        /// <param name="start">The start ID.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A view or redirect.
        /// </returns>
        /// <remarks>
        /// After tree failed attempts (=no results for a batch of IDs), the scraping is stopped (by a redirect back to Home).
        /// </remarks>
        public async Task<ActionResult> Scrape(int start = 1, CancellationToken cancellationToken = default)
        {
            const string key = "noresult";
            if (start < 1)
            {
                start = 1;
            }

            var model = new ScrapeIndexViewModel()
            {
                PreviousIndex = start,
            };

            var (count, list) = await this.tvMazeService.ScrapeBatchById(start, cancellationToken).ConfigureAwait(false);

            model.PreviousCount = list.Count;
            model.AttemptedCount = count;

            if (list.Any())
            {
                await this.showService.StoreShowList(list, null).ConfigureAwait(false);
                this.TempData.Remove(key);
            }
            else if (!cancellationToken.IsCancellationRequested)
            {
                int failcount = 0;
                if (this.TempData.ContainsKey(key))
                {
                    failcount = (int)this.TempData[key];
                    if (failcount > 3)
                    {
                        // apparently no more shows to load
                        this.logger.LogInformation("Failed {failcount} times to get a batch of data - aborting.", failcount);
                        return this.RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                }

                failcount++;
                this.TempData[key] = failcount;
                this.logger.LogInformation("Failed {failcount} times to get a batch of data (and counting).", failcount);
            }

            model.NextStartIndex = start + count;
            this.logger.LogDebug("Pausing to start scraping from {Start}.", model.NextStartIndex);
            return this.View(model);
        }
    }
}