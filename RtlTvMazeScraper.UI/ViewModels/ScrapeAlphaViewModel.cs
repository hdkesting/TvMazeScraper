// <copyright file="ScrapeAlphaViewModel.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.UI.ViewModels
{
    /// <summary>
    /// ViewModel for scraping by initial.
    /// </summary>
    public class ScrapeAlphaViewModel
    {
        /// <summary>
        /// Gets or sets the previous initial (when filled).
        /// </summary>
        /// <value>
        /// The old initial.
        /// </value>
        public string PreviousInitial { get; set; }

        /// <summary>
        /// Gets or sets the next initial.
        /// </summary>
        /// <value>
        /// The next initial.
        /// </value>
        public string NextInitial { get; set; }

        /// <summary>
        /// Gets or sets the count of shows for the <see cref="PreviousInitial"/>.
        /// </summary>
        /// <value>
        /// The previous count.
        /// </value>
        public int PreviousCount { get; set; }
    }
}
