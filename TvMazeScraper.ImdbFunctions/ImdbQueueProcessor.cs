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
    using Microsoft.Extensions.Configuration;
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
        private static readonly TimeSpan RatingMaxAge = TimeSpan.FromDays(10);
        private static readonly TimeSpan OmdbDelay = TimeSpan.FromHours(4);

        /// <summary>
        /// Processes the specified queue item.
        /// </summary>
        /// <param name="queueItem">The queue item that triggered this function.</param>
        /// <param name="tableCache">The Azure table cache with known ratings.</param>
        /// <param name="incomingQueueClient">The queue client for incoming requests.</param>
        /// <param name="resultQueueClient">The queue client for the rating result.</param>
        /// <param name="log">The log to write messages to.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        [FunctionName("ImdbQueueProcessor")]
        public static async Task Run(
            [QueueTrigger("imdbratingqueue", Connection = "imdbrating")]
            CloudQueueMessage queueItem,
            [Table("imdbratingcache", Connection = "imdbrating")]
            CloudTable tableCache,
            [Queue("imdbratingqueue", Connection = "imdbrating")]
            CloudQueue incomingQueueClient,
            [Queue("ratingresultqueue", Connection = "imdbrating")]
            CloudQueue resultQueueClient,
            ILogger log,
            ExecutionContext context)
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
                await SendRatingMessage(resultQueueClient, request.ShowId, rating.Rating).ConfigureAwait(false);

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
                await RequeueMessage(incomingQueueClient, json).ConfigureAwait(false);
            }
            else
            {
                // https://stackoverflow.com/questions/43556311/reading-settings-from-a-azure-function
                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                string apiKey = config["omdbapikey"];
                var (status, newrating) = await QueryOmdbForRating(apiKey, request.ImdbId).ConfigureAwait(false);

                if (status == HttpStatusCode.OK)
                {
                    log.LogInformation($"Found a rating of {newrating} for {request.ImdbId}.");

                    await StoreRatingInTable(tableCache, request.ImdbId, newrating).ConfigureAwait(false);
                    await SendRatingMessage(resultQueueClient, request.ShowId, rating.Rating).ConfigureAwait(false);
                }
                else if (status == HttpStatusCode.NotFound)
                {
                    log.LogInformation($"Found NO rating for {request.ImdbId}. Ignoring.");
                }
                else
                {
                    // apparently recieved an error, probably "too much"
                    log.LogWarning($"Got a response of {status} ({(int)status}) for {request.ImdbId}. Blocking for {OmdbDelay.TotalHours} hours.");
                    await SetOmdbBlocked(tableCache).ConfigureAwait(false);
                    await RequeueMessage(incomingQueueClient, json).ConfigureAwait(false);
                }
            }

            log.LogInformation("Done processing this item.");
        }

        private static async Task RequeueMessage(CloudQueue queueClient, string json)
        {
            var cqm = new CloudQueueMessage(json);
            await queueClient.AddMessageAsync(message: cqm, timeToLive: null, initialVisibilityDelay: OmdbDelay, options: null, operationContext: null).ConfigureAwait(false);
        }

        private static async Task<(HttpStatusCode status, decimal rating)> QueryOmdbForRating(string apiKey, string imdbId)
        {
            var uri = new Uri($"?apikey={apiKey}&i={imdbId}", UriKind.Relative);
            var (status, text) = await GetJsonText(uri).ConfigureAwait(false);

            if (status != HttpStatusCode.OK)
            {
                // either "404 not found" or "429 too much"(?)
                return (status, 0);
            }

            dynamic json = JObject.Parse(text);

            string response = json.Response;

            if (response == "False")
            {
                return (HttpStatusCode.NotFound, 0);
            }

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
        /// Determines whether it is already known that OMDb is blocked ("circuit breaker").
        /// </summary>
        /// <param name="table">The Azure Table to read from.</param>
        /// <returns>A value indicating whether OMDb is considered blocked.</returns>
        private static async Task<bool> OmdbIsBlocked(CloudTable table)
        {
            var getOperation = TableOperation.Retrieve<OmdbBlock>(OmdbBlock.ConfigPartitionKey, OmdbBlock.OmdbBlockKey);

            var result = await table.ExecuteAsync(getOperation).ConfigureAwait(false);

            if (result.Result is null)
            {
                // no "blocked" record found, so not blocked
                return false;
            }

            return ((OmdbBlock)result.Result).BlockedUntil > DateTimeOffset.Now;
        }

        /// <summary>
        /// Sets the omdb service as blocked for some time.
        /// </summary>
        /// <param name="table">The Azure Table to store in.</param>
        /// <returns>A Task.</returns>
        private static async Task SetOmdbBlocked(CloudTable table)
        {
            var block = new OmdbBlock
            {
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
        private static async Task SendRatingMessage(CloudQueue resultQueueClient, int showId, decimal rating)
        {
            // create JSON
            var json = $"{{ \"showid\": {showId}, \"rating\": {rating} }}";

            // add to the queue, visible immediately
            var cqm = new CloudQueueMessage(json);
            await resultQueueClient.AddMessageAsync(message: cqm, timeToLive: null, initialVisibilityDelay: null, options: null, operationContext: null).ConfigureAwait(false);
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
            var getOperation = TableOperation.Retrieve<RatingCacheItem>(RatingCacheItem.RatingsPartitionKey, imdbId);

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

            var operation = TableOperation.InsertOrReplace(cacheItem);

            await table.ExecuteAsync(operation).ConfigureAwait(false);
        }
    }
}
