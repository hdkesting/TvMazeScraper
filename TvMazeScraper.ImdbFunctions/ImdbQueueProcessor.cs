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
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.Table;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using TvMazeScraper.ImdbFunctions.Model;
    using TvMazeScraper.ImdbFunctions.Services;

    /// <summary>
    /// Processes queue items by looking up the imdb rating using the omdb service. Will store the found rating in the table cache.
    /// </summary>
    /// <remarks>
    /// Will requeue the item when the omdb service has reached its daily limit.
    /// If the show was not found, store a -1 to prevent asking again quickly.
    /// </remarks>
    public static class ImdbQueueProcessor
    {
        /// <summary>
        /// Processes the specified queue item.
        /// </summary>
        /// <param name="queueItem">The queue item that triggered this function.</param>
        /// <param name="tableCache">The table cache.</param>
        /// <param name="queueClient">The queue client.</param>
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
            CloudQueue queueClient,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"C# Queue trigger to process: {queueItem}");

            var cacheTableService = new CacheTableService(tableCache);
            var queueService = new QueueService(queueClient);

            var json = queueItem.AsString;
            var request = JsonConvert.DeserializeObject<RatingRequest>(json);

            log.LogInformation($"Trying to get rating for {request.ImdbId}/{request.ShowId}.");

            var rating = await cacheTableService.GetRating(request.ImdbId).ConfigureAwait(false);

            bool earlyexit = false;
            if (!(rating is null) && rating.ScaledRating > 0)
            {
                // there is a useful rating (I don't accept 0.0 as rating)
                log.LogInformation($"Got a rating of {rating.ScaledRating} from {rating.Date}.");

                if (CacheTableService.IsRecentEnough(rating.Date))
                {
                    // I got a recent rating, so there is no need to request it again.
                    log.LogInformation("Done quickly processing this item.");
                    earlyexit = true;
                }
                else
                {
                    log.LogInformation("Rating was old, so try and get fresh one.");
                }
            }

            if (!earlyexit)
            {
                if (await cacheTableService.OmdbIsBlocked().ConfigureAwait(false))
                {
                    log.LogWarning($"OMDb was blocked, postponing request for {CacheTableService.OmdbDelay.TotalHours} hours.");

                    // re-enter in queue with updated timeout
                    await queueService.RequeueMessage(json).ConfigureAwait(false);
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

                        if (newrating > 0m)
                        {
                            await cacheTableService.StoreRating(request.ImdbId, newrating).ConfigureAwait(false);
                        }
                    }
                    else if (status == HttpStatusCode.NotFound)
                    {
                        log.LogInformation($"Found NO rating for {request.ImdbId}. Storing -1 to prevent asking again.");
                        await cacheTableService.StoreRating(request.ImdbId, -1m).ConfigureAwait(false);
                    }
                    else
                    {
                        // apparently recieved an error, probably "too much"
                        log.LogWarning($"Got a response of {status} ({(int)status}) for {request.ImdbId}. Blocking for {CacheTableService.OmdbDelay.TotalHours} hours.");
                        await cacheTableService.SetOmdbBlocked().ConfigureAwait(false);
                        await queueService.RequeueMessage(json).ConfigureAwait(false);
                    }
                }
            }

            // delete the current message as we are done with it (either processed or requeued as new message)
            await queueService.DeleteMessageAsync(queueItem).ConfigureAwait(false);
            log.LogInformation("Done processing this item.");
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
    }
}
