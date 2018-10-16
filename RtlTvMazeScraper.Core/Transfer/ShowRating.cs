// <copyright file="ShowRating.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Core.Transfer
{
    /// <summary>
    /// The rating for one show.
    /// </summary>
    public sealed class ShowRating
    {
        /// <summary>
        /// Gets or sets the internal (TvMaze) show identifier.
        /// </summary>
        /// <value>
        /// The show identifier.
        /// </value>
        public int ShowId { get; set; }

        /// <summary>
        /// Gets or sets the IMDb rating.
        /// </summary>
        /// <value>
        /// The rating.
        /// </value>
        public decimal Rating { get; set; }
    }
}
