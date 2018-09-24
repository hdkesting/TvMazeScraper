// <copyright file="ScrapeBatchResult.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Transfer
{
    using System.Collections.Generic;
    using RtlTvMazeScraper.Core.DTO;

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
            this.Shows = new List<ShowDto>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrapeBatchResult"/> class.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="shows">The shows.</param>
        public ScrapeBatchResult(int count, List<ShowDto> shows)
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
        /// Gets the actual shows retrieved.
        /// </summary>
        /// <value>
        /// The shows.
        /// </value>
        public List<ShowDto> Shows { get; }

        /// <summary>
        /// Deconstructs this instance.
        /// </summary>
        /// <param name="count">The number of shows tried.</param>
        /// <param name="shows">The shows.</param>
        public void Deconstruct(out int count, out List<ShowDto> shows)
        {
            count = this.NumberOfShowsTried;
            shows = this.Shows;
        }
    }
}
