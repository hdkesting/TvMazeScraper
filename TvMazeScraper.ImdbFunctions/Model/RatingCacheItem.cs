// <copyright file="RatingCacheItem.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.ImdbFunctions.Model
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// The value stored in the rating table.
    /// </summary>
    public class RatingCacheItem : TableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RatingCacheItem"/> class.
        /// </summary>
        /// <remarks>
        /// The default constructor is required.
        /// </remarks>
        public RatingCacheItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingCacheItem"/> class.
        /// </summary>
        /// <param name="imdbId">The imdb identifier.</param>
        /// <param name="rating">The rating.</param>
        public RatingCacheItem(string imdbId, decimal rating)
        {
            this.ImdbId = imdbId;
            this.Rating = rating;
            this.RetrievalDate = DateTimeOffset.Now;
        }

        /// <summary>
        /// Gets or sets the IMDb identifier.
        /// </summary>
        /// <value>
        /// The imdb identifier.
        /// </value>
        public string ImdbId { get; set; }

        /// <summary>
        /// Gets or sets the IMDb rating.
        /// </summary>
        /// <value>
        /// The rating.
        /// </value>
        public decimal Rating { get; set; }

        /// <summary>
        /// Gets or sets the date the rating was retrieved.
        /// </summary>
        /// <value>
        /// The retrieval date.
        /// </value>
        public DateTimeOffset RetrievalDate { get; set; }
    }
}
