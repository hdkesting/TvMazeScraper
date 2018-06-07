// <copyright file="ScrapeBatchResult.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Transfer
{
    using System.Collections.Generic;
    using RtlTvMazeScraper.Core.Model;

    /// <summary>
    /// Result of scraping a batch of shows.
    /// </summary>
    public class ScrapeBatchResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScrapeBatchResult"/> class.
        /// </summary>
        public ScrapeBatchResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrapeBatchResult"/> class.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="shows">The shows.</param>
        public ScrapeBatchResult(int count, List<Show> shows)
        {
            this.NumberOfShowsTried = count;
            this.Shows = shows;
        }

        /// <summary>
        /// Gets or sets the number of shows tried in this batch.
        /// </summary>
        /// <value>
        /// The number of shows tried.
        /// </value>
        public int NumberOfShowsTried { get; set; }

        /// <summary>
        /// Gets or sets the actual shows retrieved.
        /// </summary>
        /// <value>
        /// The shows.
        /// </value>
        public List<Show> Shows { get; set; }

        /// <summary>
        /// Deconstructs this instance.
        /// </summary>
        /// <param name="count">The number of shows tried.</param>
        /// <param name="shows">The shows.</param>
        public void Deconstruct(out int count, out List<Show> shows)
        {
            count = this.NumberOfShowsTried;
            shows = this.Shows;
        }
    }
}
