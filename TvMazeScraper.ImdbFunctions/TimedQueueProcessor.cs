// <copyright file="TimedQueueProcessor.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.ImdbFunctions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

    public static class TimedQueueProcessor
    {
        private static readonly TimeSpan RatingMaxAge = TimeSpan.FromDays(10);
        private static readonly TimeSpan OmdbDelay = TimeSpan.FromHours(4);

        [FunctionName("TimedQueueProcessor")]
        public static async Task Run(
            [TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
            [Table("imdbratingcache", Connection = "imdbrating")]
            CloudTable tableCache,
            [Queue("imdbratingqueue", Connection = "imdbrating")]
            CloudQueue incomingQueueClient,
            ExecutionContext context,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (await OmdbIsBlocked(tableCache).ConfigureAwait(false))
            {
                log.LogInformation("Execution skipped because OMDB was known to be blocked.");
                return;
            }

            var toProcess = await GetQueuedRequests(incomingQueueClient).ConfigureAwait(false);

            if (!toProcess.Any())
            {
                log.LogInformation("Nothing to do.");
                return;
            }

            var config = new ConfigurationBuilder()
                            .SetBasePath(context.FunctionAppDirectory)
                            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables()
                            .Build();

            string apiKey = config["omdbapikey"];

            var remaining = await ProcessList(toProcess, apiKey, tableCache, log).ConfigureAwait(false);

            if (remaining.Any())
            {
                await RequeueMessages(incomingQueueClient, remaining).ConfigureAwait(false);
            }
        }

        private static async Task<IList<RatingRequest>> ProcessList(
            List<RatingRequest> ratingRequests,
            string apiKey,
            CloudTable tableCache,
            ILogger log)
        {
            var requests = ratingRequests.ToList();

            while (requests.Any())
            {
                var request = requests.First();
                requests.RemoveAt(0);

                // https://stackoverflow.com/questions/43556311/reading-settings-from-a-azure-function
                var (status, newrating) = await QueryOmdbForRating(apiKey, request.ImdbId).ConfigureAwait(false);

                if (status == HttpStatusCode.OK)
                {
                    log.LogInformation($"Found a rating of {newrating} for {request.ImdbId}.");

                    if (newrating > 0m)
                    {
                        await StoreRatingInTable(tableCache, request.ImdbId, newrating).ConfigureAwait(false);
                    }
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

                    // not handled, so re-add
                    requests.Add(request);
                    break;
                }
            }

            return requests;
        }

        private static async Task RequeueMessages(CloudQueue queueClient, IList<RatingRequest> ratingRequests)
        {
            foreach (var msg in ratingRequests)
            {
                string json = JsonConvert.SerializeObject(msg);
                var cqm = new CloudQueueMessage(json);
                await queueClient.AddMessageAsync(message: cqm, timeToLive: null, initialVisibilityDelay: OmdbDelay, options: null, operationContext: null).ConfigureAwait(false);
            }
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
            using (var httpClient = HttpClientFactory.Create())
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

        private static async Task<List<RatingRequest>> GetQueuedRequests(CloudQueue incomingQueueClient)
        {
            const int maxPossibleAmount = 32;

            var request = await incomingQueueClient.GetMessagesAsync(maxPossibleAmount).ConfigureAwait(false);

            var list = new List<RatingRequest>();
            foreach (var msg in request)
            {
                var rr = JsonConvert.DeserializeObject<RatingRequest>(msg.AsString);
                await incomingQueueClient.DeleteMessageAsync(msg).ConfigureAwait(false);
            }

            return list;
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

    }
}
