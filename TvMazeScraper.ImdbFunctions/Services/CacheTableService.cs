// <copyright file="CacheTableService.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.ImdbFunctions.Services
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.WindowsAzure.Storage.Table;
    using TvMazeScraper.ImdbFunctions.Model;

    /// <summary>
    /// Interacts with the Table cache.
    /// </summary>
    public class CacheTableService
    {
        /// <summary>
        /// The delay to use before a new request is attempted.
        /// </summary>
        public static readonly TimeSpan OmdbDelay = TimeSpan.FromHours(4);
        private static readonly TimeSpan RatingMaxAge = TimeSpan.FromDays(10);
        private readonly CloudTable tableCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheTableService"/> class.
        /// </summary>
        /// <param name="tableCache">The table cache.</param>
        public CacheTableService(
            [Table("imdbratingcache", Connection = "imdbrating")]
            CloudTable tableCache)
        {
            this.tableCache = tableCache;
        }

        /// <summary>
        /// Determines whether the specified retrieval date of the rating is recent enough so that we don't need to request it again.
        /// </summary>
        /// <param name="retrievalDate">The retrieval date.</param>
        /// <returns>
        ///   <c>true</c> if the specified retrieval date is recent enough; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsRecentEnough(DateTimeOffset retrievalDate)
        {
            return DateTimeOffset.Now - retrievalDate < RatingMaxAge;
        }

        /// <summary>
        /// Gets the rating for the specified IMDB Id, if known.
        /// </summary>
        /// <param name="imdbId">The imdb identifier.</param>
        /// <returns>A Task of <see cref="RatingCacheItem"/>.</returns>
        public async Task<RatingCacheItem> GetRating(string imdbId)
        {
            var getOperation = TableOperation.Retrieve<RatingCacheItem>(RatingCacheItem.GetPartitionKeyForImdbId(imdbId), imdbId);

            var result = await this.tableCache.ExecuteAsync(getOperation).ConfigureAwait(false);

            // result may be null!
            return (RatingCacheItem)result.Result;
        }

        /// <summary>
        /// Stores (inserts or overwrites) the rating in table.
        /// </summary>
        /// <param name="imdbId">The imdb identifier.</param>
        /// <param name="rating">The rating.</param>
        /// <returns>A Task.</returns>
        public async Task StoreRating(string imdbId, decimal rating)
        {
            var cacheItem = new RatingCacheItem(imdbId, rating);

            var operation = TableOperation.InsertOrReplace(cacheItem);

            await this.tableCache.ExecuteAsync(operation).ConfigureAwait(false);
        }

        /// <summary>
        /// Determines whether it is already known that OMDb is blocked ("circuit breaker").
        /// </summary>
        /// <returns>A value indicating whether OMDb is considered blocked.</returns>
        public async Task<bool> OmdbIsBlocked()
        {
            var getOperation = TableOperation.Retrieve<OmdbBlock>(OmdbBlock.ConfigPartitionKey, OmdbBlock.OmdbBlockKey);

            var result = await this.tableCache.ExecuteAsync(getOperation).ConfigureAwait(false);

            if (result.Result is null)
            {
                // no "blocked" record found, so not blocked
                return false;
            }

            return ((OmdbBlock)result.Result).Date > DateTimeOffset.Now;
        }

        /// <summary>
        /// Sets the omdb service as blocked for some time.
        /// </summary>
        /// <param name="table">The Azure Table to store in.</param>
        /// <returns>A Task.</returns>
        public async Task SetOmdbBlocked()
        {
            var block = new OmdbBlock
            {
                Date = DateTimeOffset.Now + OmdbDelay,
            };
            var operation = TableOperation.InsertOrReplace(block);
            await this.tableCache.ExecuteAsync(operation).ConfigureAwait(false);
        }
    }
}
