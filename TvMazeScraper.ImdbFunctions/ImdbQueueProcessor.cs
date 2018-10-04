// <copyright file="ImdbQueueProcessor.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.ImdbFunctions
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.Table;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using TvMazeScraper.ImdbFunctions.Model;

    /// <summary>
    /// Processes queue items by looking up the imdb rating using the omdb service.
    /// Will requeue the item when the omdb service has reached its daily limit.
    /// </summary>
    public static class ImdbQueueProcessor
    {
        private const string RatingsPartitionKey = "ratings";
        private const string ConfigPartitionKey = "config";
        private const string OmdbBlockKey = "omdb-block";
        private static readonly TimeSpan RatingMaxAge = TimeSpan.FromDays(10);
        private static readonly TimeSpan OmdbDelay = TimeSpan.FromHours(4);

        /// <summary>
        /// Processes the specified queue item.
        /// </summary>
        /// <param name="queueItem">The queue item that triggered this function.</param>
        /// <param name="tableCache">The Azure table cache with known ratings.</param>
        /// <param name="queueClient">The queue client.</param>
        /// <param name="log">The log to write messages to.</param>
        [FunctionName("ImdbQueueProcessor")]
        public static async void Run(
            [QueueTrigger("imdbratingqueue", Connection = "imdbrating")]
            CloudQueueMessage queueItem,
            [Table("imdbratingcache", Connection = "imdbrating")]
            CloudTable tableCache,
            [Queue("imdbratingqueue", Connection = "imdbrating")]
            CloudQueue queueClient,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger to process: {queueItem}");

            var json = queueItem.AsString;
            var request = JsonConvert.DeserializeObject<RatingRequest>(json);

            log.LogInformation($"Trying to get rating for {request.ImdbId}/{request.ShowId}.");

            var rating = await GetRatingFromTable(tableCache, request.ImdbId).ConfigureAwait(false);

            if (!(rating is null))
            {
                log.LogInformation($"Got a rating of {rating.Rating} from {rating.RetrievalDate}.");

                // I got *some* rating. Just send it anyway, then maybe get a more recent one.
                await SendRatingMessage(request.ShowId, rating.Rating).ConfigureAwait(false);

                if (IsRecentEnough(rating.RetrievalDate))
                {
                    // I got a recent rating, so there is no need to request it again.
                    log.LogInformation("Done quickly processing this item.");
                    return;
                }

                log.LogInformation("Rating was old, so try and get fresh one.");
            }

            if (await OmdbIsBlocked(tableCache).ConfigureAwait(false))
            {
                log.LogWarning($"OMDb was blocked, postponing request for {OmdbDelay.TotalHours} hours.");

                // re-enter in queue with updated timeout
                await RequeueMessage(queueClient, json).ConfigureAwait(false);
            }
            else
            {
                var ratingResponse = await QueryOmdbForRating(request.ImdbId).ConfigureAwait(false);

                if (ratingResponse.status == HttpStatusCode.OK)
                {
                    log.LogInformation($"Found a rating of {ratingResponse.rating} for {request.ImdbId}.");

                    await StoreRatingInTable(tableCache, request.ImdbId, ratingResponse.rating).ConfigureAwait(false);
                    await SendRatingMessage(request.ShowId, rating.Rating).ConfigureAwait(false);
                }
                else if (ratingResponse.status == HttpStatusCode.NotFound)
                {
                    log.LogInformation($"Found NO rating for {request.ImdbId}. Ignoring.");
                }
                else
                {
                    // apparently recieved an error, probably "too much"
                    log.LogWarning($"Got a response of {ratingResponse.status} ({(int)ratingResponse.status}) for {request.ImdbId}. Blocking for {OmdbDelay.TotalHours} hours.");
                    await SetOmdbBlocked(tableCache).ConfigureAwait(false);
                    await RequeueMessage(queueClient, json).ConfigureAwait(false);
                }
            }

            log.LogInformation("Done processing this item.");
        }

        private static async Task RequeueMessage(CloudQueue queueClient, string json)
        {
            var cqm = new CloudQueueMessage(json);
            await queueClient.AddMessageAsync(message: cqm, timeToLive: null, initialVisibilityDelay: OmdbDelay, options: null, operationContext: null).ConfigureAwait(false);
        }

        private static async Task<(HttpStatusCode status, decimal rating)> QueryOmdbForRating(string imdbId)
        {
            string apiKey = "40204b22"; // TODO read from config!
            var uri = new Uri($"?apikey={apiKey}&i={imdbId}", UriKind.Relative);
            var response = await GetJsonText(uri).ConfigureAwait(false);

            if (response.status != HttpStatusCode.OK)
            {
                // either "404 not found" or "429 too much"(?)
                return (response.status, 0);
            }

            dynamic json = JObject.Parse(response.text);

            decimal rating = json.imdbRating;

            return (HttpStatusCode.OK, rating);
        }

        private static async Task<(HttpStatusCode status, string text)> GetJsonText(Uri relativeUri)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://www.omdbapi.com"); // TODO read from config?

                var response = await httpClient.GetAsync(relativeUri).ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return (HttpStatusCode.OK, text);
                }
                else
                {
                    return (response.StatusCode, string.Empty);
                }
            }
        }

        /// <summary>
        /// Determines whether it is already known that OMDb is blocked.
        /// </summary>
        /// <param name="tableCache">The table cache.</param>
        /// <returns>A value indicating whether OMDb is considered blocked.</returns>
        private static async Task<bool> OmdbIsBlocked(CloudTable tableCache)
        {
            var getOperation = TableOperation.Retrieve<OmdbBlock>(ConfigPartitionKey, OmdbBlockKey);

            var result = await tableCache.ExecuteAsync(getOperation).ConfigureAwait(false);

            if (result.Result is null)
            {
                // no "blocked" record found, so not blocked
                return false;
            }

            return ((OmdbBlock)result.Result).BlockedUntil > DateTimeOffset.Now;
        }

        private static async Task SetOmdbBlocked(CloudTable table)
        {
            var block = new OmdbBlock
            {
                PartitionKey = ConfigPartitionKey,
                RowKey = OmdbBlockKey,
                BlockedUntil = DateTimeOffset.Now + OmdbDelay,
            };
            var operation = TableOperation.InsertOrReplace(block);
            await table.ExecuteAsync(operation).ConfigureAwait(false);
        }

        /// <summary>
        /// Determines whether the specified retrieval date of the rating is recent enough so that we don't need to request it again.
        /// </summary>
        /// <param name="retrievalDate">The retrieval date.</param>
        /// <returns>
        ///   <c>true</c> if the specified retrieval date is recent enough; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsRecentEnough(DateTimeOffset retrievalDate)
        {
            return DateTimeOffset.Now - retrievalDate < RatingMaxAge;
        }

        /// <summary>
        /// Sends the rating over the message bus to the website.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <param name="rating">The rating.</param>
        private static Task SendRatingMessage(int showId, decimal rating)
        {
            // TODO add parameter for message bus
            // TODO use that service bus to send the message
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the rating from the Azure Table cache.
        /// </summary>
        /// <param name="table">The table cache.</param>
        /// <param name="imdbId">The imdb identifier.</param>
        /// <returns>
        /// The <see cref="RatingCacheItem" /> found, or <c>null</c>.
        /// </returns>
        private static async Task<RatingCacheItem> GetRatingFromTable(CloudTable table, string imdbId)
        {
            var getOperation = TableOperation.Retrieve<RatingCacheItem>(RatingsPartitionKey, imdbId);

            var result = await table.ExecuteAsync(getOperation).ConfigureAwait(false);

            // result may be null!
            return (RatingCacheItem)result.Result;
        }

        /// <summary>
        /// Stores (inserts or overwrites) the rating in table.
        /// </summary>
        /// <param name="table">The table cache.</param>
        /// <param name="imdbId">The imdb identifier.</param>
        /// <param name="rating">The rating.</param>
        /// <returns>A Task.</returns>
        private static async Task StoreRatingInTable(CloudTable table, string imdbId, decimal rating)
        {
            var cacheItem = new RatingCacheItem(imdbId, rating);
            cacheItem.PartitionKey = RatingsPartitionKey;
            cacheItem.RowKey = imdbId;

            var operation = TableOperation.InsertOrReplace(cacheItem);

            await table.ExecuteAsync(operation).ConfigureAwait(false);
        }
    }
}
