// <copyright file="ScrapeIndexViewModel.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.UI.ViewModels
{
    /// <summary>
    /// ViewModel for scrape-by-index.
    /// </summary>
    public class ScrapeIndexViewModel
    {
        /// <summary>
        /// Gets or sets the next start index.
        /// </summary>
        /// <value>
        /// The start index.
        /// </value>
        public int NextStartIndex { get; set; }

        /// <summary>
        /// Gets or sets the previous start index.
        /// </summary>
        /// <value>
        /// The index of the previous.
        /// </value>
        public int PreviousIndex { get; set; }

        /// <summary>
        /// Gets or sets the previous count.
        /// </summary>
        /// <value>
        /// The previous count.
        /// </value>
        public int PreviousCount { get; set; }

        /// <summary>
        /// Gets or sets the attempted count.
        /// </summary>
        /// <value>
        /// The attempted count.
        /// </value>
        public int AttemptedCount { get; set; }
    }
}
