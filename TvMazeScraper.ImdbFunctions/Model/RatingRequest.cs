// <copyright file="RatingRequest.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.ImdbFunctions.Model
{
    /// <summary>
    /// Request for a rating, as stored in the queu.
    /// </summary>
    public class RatingRequest
    {
        /// <summary>
        /// Gets or sets the imdb identifier.
        /// </summary>
        /// <value>
        /// The imdb identifier.
        /// </value>
        public string ImdbId { get; set; }

        /// <summary>
        /// Gets or sets the (TvMaze) show identifier.
        /// </summary>
        /// <value>
        /// The show identifier.
        /// </value>
        public int ShowId { get; set; }
    }
}
