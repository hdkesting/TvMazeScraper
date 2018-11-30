// <copyright file="QueryRating.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.ImdbFunctions
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.Table;
    using TvMazeScraper.ImdbFunctions.Model;
    using TvMazeScraper.ImdbFunctions.Services;

    /// <summary>
    /// Query for rating. Responds with a rating (status 200) or  status 204 (no content) if not known.
    /// </summary>
    public static class QueryRating
    {
        /// <summary>
        /// Runs the specified request.
        /// </summary>
        /// <param name="req">The incoming request.</param>
        /// <param name="tableCache">The table cache.</param>
        /// <param name="queueClient">The queue client.</param>
        /// <param name="log">The logger.</param>
        /// <returns>
        /// A rating, if found.
        /// </returns>
        [FunctionName("QueryRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            [Table("imdbratingcache", Connection = "imdbrating")]
            CloudTable tableCache,
            [Queue("imdbratingqueue", Connection = "imdbrating")]
            CloudQueue queueClient,
            ILogger log)
        {
            string imdbId = req.Query["imdbid"];
            log.LogInformation($"C# HTTP trigger function {nameof(QueryRating)} processed a request for '{imdbId}'.");

            if (string.IsNullOrWhiteSpace(imdbId))
            {
                return new BadRequestResult();
            }

            var cacheTableService = new CacheTableService(tableCache);
            var queueService = new QueueService(queueClient);

            var rating = await cacheTableService.GetRating(imdbId).ConfigureAwait(false);

            if (rating is null)
            {
                // no rating, so add to queue and terurn nothing. Come back later, maybe there is info then.
                log.LogInformation("No rating found, so request will be queued.");
                await queueService.QueueMessage(new RatingRequest { ImdbId = imdbId }).ConfigureAwait(false);

                return new NoContentResult();
            }
            else
            {
                // there is a rating (might be "unknown")
                log.LogInformation($"Got a rating of {rating.Rating} from {rating.RetrievalDate}.");

                if (!CacheTableService.IsRecentEnough(rating.RetrievalDate))
                {
                    log.LogInformation("Rating was old, so try and get fresh one.");
                    await queueService.QueueMessage(new RatingRequest { ImdbId = imdbId }).ConfigureAwait(false);
                }

                return new OkObjectResult(rating.Rating);
            }
        }
    }
}
