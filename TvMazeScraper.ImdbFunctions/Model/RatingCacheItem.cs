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
        /// The partition key for the ratings.
        /// </summary>
        private const string RatingsPartitionKey = "ratings";

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingCacheItem"/> class.
        /// </summary>
        /// <remarks>
        /// The default constructor is required.
        /// </remarks>
        public RatingCacheItem()
        {
            this.PartitionKey = RatingsPartitionKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingCacheItem"/> class.
        /// </summary>
        /// <param name="imdbId">The imdb identifier.</param>
        /// <param name="rating">The rating.</param>
        public RatingCacheItem(string imdbId, decimal rating)
            : this()
        {
            this.PartitionKey = GetPartitionKeyForImdbId(imdbId);
            this.ImdbId = this.RowKey = imdbId;
            this.ScaledRating = (int)(rating * 100);
            this.Date = DateTimeOffset.Now;
        }

        /// <summary>
        /// Gets or sets the IMDb identifier.
        /// </summary>
        /// <value>
        /// The imdb identifier.
        /// </value>
        public string ImdbId { get; set; }

        /// <summary>
        /// Gets or sets the scaled (0..100) IMDb rating.
        /// </summary>
        /// <value>
        /// The rating.
        /// </value>
        public int ScaledRating { get; set; }

        /// <summary>
        /// Gets or sets the date the rating was retrieved.
        /// </summary>
        /// <value>
        /// The retrieval date.
        /// </value>
        public DateTimeOffset Date { get; set; }

        /// <summary>
        /// Gets the partition key for the specified IMDB id.
        /// </summary>
        /// <param name="imdbId">The imdb identifier.</param>
        /// <returns>A key value.</returns>
        public static string GetPartitionKeyForImdbId(string imdbId)
        {
            return imdbId?.Substring(0, 5);
        }
    }
}
