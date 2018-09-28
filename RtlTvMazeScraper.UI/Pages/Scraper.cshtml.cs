// <copyright file="Scraper.cshtml.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.UI.Pages
{
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using TvMazeScraper.Core.Interfaces;

#pragma warning disable SA1649 // File name should match first type name
    /// <summary>
    /// Model class for scraper page.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.RazorPages.PageModel" />
    public class ScraperModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly IShowService showService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScraperModel"/> class.
        /// </summary>
        /// <param name="showService">The show service.</param>
        public ScraperModel(IShowService showService)
        {
            this.showService = showService;
        }

        /// <summary>
        /// Gets the maximum index of the show.
        /// </summary>
        /// <value>
        /// The maximum index of the show.
        /// </value>
        public int MaxShowIndex { get; private set; }

        /// <summary>
        /// Called when [get].
        /// </summary>
        public async void OnGet()
        {
            this.MaxShowIndex = await this.showService.GetMaxShowId().ConfigureAwait(false);
        }
    }
}