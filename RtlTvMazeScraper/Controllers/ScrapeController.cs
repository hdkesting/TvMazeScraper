// <copyright file="ScrapeController.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using RtlTvMazeScraper.Core.Interfaces;

    /// <summary>
    /// A controller that performs the scraping of the TvMaze site.
    /// </summary>
    /// <seealso cref="System.Web.Mvc.Controller" />
    public class ScrapeController : Controller
    {
        private readonly IShowService showService;
        private readonly ITvMazeService tvMazeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrapeController" /> class.
        /// </summary>
        /// <param name="showService">The show service.</param>
        /// <param name="tvMazeService">The tv maze service.</param>
        public ScrapeController(
            IShowService showService,
            ITvMazeService tvMazeService)
        {
            this.showService = showService;
            this.tvMazeService = tvMazeService;
        }

        /// <summary>
        /// Shows the default page.
        /// </summary>
        /// <returns>A View.</returns>
        public async Task<ActionResult> Index()
        {
            var max = await this.showService.GetMaxShowId();
            this.ViewBag.Max = max;

            return this.View();
        }

        /// <summary>
        /// Scrapes by searching for an initial.
        /// </summary>
        /// <param name="initial">The initial to search for.</param>
        /// <returns>A View or a redirect.</returns>
        public async Task<ActionResult> ScrapeAlpha(string initial)
        {
            if (string.IsNullOrEmpty(initial))
            {
                initial = "a";
            }
            else
            {
                initial = initial.Substring(0, 1).ToLowerInvariant();
                if (initial.CompareTo("a") < 0)
                {
                    initial = "a";
                }
                else if (initial.CompareTo("z") > 0)
                {
                    initial = "z";
                }

                // perform scrape
                var list = await this.tvMazeService.ScrapeShowsBySearch(initial);

                await this.showService.StoreShowList(list, id => this.tvMazeService.ScrapeCastMembers(id));

                if (initial.StartsWith("z"))
                {
                    // done!
                    return this.RedirectToAction(nameof(this.Index));
                }

                // setup for next initial
                initial = ((char)(initial[0] + 1)).ToString();
            }

            this.ViewBag.Initial = initial;

            return this.View();
        }

        /// <summary>
        /// Scrapes one batch from the specified start ID (defaults to 1).
        /// </summary>
        /// <remarks>
        /// After tree failed attempts (=no results for a batch of IDs), the scraping is stopped (by a redirect back to Home).
        /// </remarks>
        /// <param name="start">The start ID.</param>
        /// <returns>A view or redirect.</returns>
        public async Task<ActionResult> Scrape(int start = 1)
        {
            const string key = "noresult";
            if (start < 1)
            {
                start = 1;
            }

            var (count, list) = await this.tvMazeService.ScrapeById(start);

            if (list.Any())
            {
                await this.showService.StoreShowList(list, null);
                this.TempData.Remove(key);
            }
            else
            {
                int failcount = 0;
                if (this.TempData.ContainsKey(key))
                {
                    failcount = (int)this.TempData[key];
                    if (failcount > 3)
                    {
                        // apparently no more shows to load
                        return this.RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                }

                failcount++;
                this.TempData[key] = failcount;
            }

            this.ViewBag.Start = start + count;
            return this.View();
        }
    }
}