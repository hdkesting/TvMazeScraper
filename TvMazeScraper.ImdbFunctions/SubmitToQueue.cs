// <copyright file="SubmitToQueue.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.ImdbFunctions
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;
    using TvMazeScraper.ImdbFunctions.Model;

    /// <summary>
    /// Submit an entry to the queue, to get the rating eventually.
    /// </summary>
    public static class SubmitToQueue
    {
        /// <summary>
        /// Runs the specified request.
        /// </summary>
        /// <param name="req">The request.</param>
        /// <param name="queueClient">The queue client.</param>
        /// <param name="log">The log.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        [FunctionName("SubmitToQueue")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            [Queue("imdbratingqueue", Connection = "imdbrating")]
            CloudQueue queueClient,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string imdbId = req.Query["imdbid"];
            if (!string.IsNullOrEmpty(imdbId)
                && int.TryParse(req.Query["showid"], out int showId)
                && showId > 0)
            {
                log.LogInformation($"Entering [{imdbId},{showId}] into queue.");
                await EnqueueMessage(queueClient, imdbId, showId).ConfigureAwait(false);

                log.LogInformation("Success!");
                return new OkResult();
            }

            log.LogInformation($"Bad request: imdb-id={req.Query["imdbid"]}, show-id={req.Query["showid"]}.");
            return new BadRequestResult();
        }

        private static async Task EnqueueMessage(CloudQueue queueClient, string imdbId, int showId)
        {
            var entry = new RatingRequest() { ImdbId = imdbId, ShowId = showId };
            var json = JsonConvert.SerializeObject(entry);
            var cqm = new CloudQueueMessage(json);
            await queueClient.AddMessageAsync(message: cqm, timeToLive: null, initialVisibilityDelay: null, options: null, operationContext: null).ConfigureAwait(false);
        }
    }
}
