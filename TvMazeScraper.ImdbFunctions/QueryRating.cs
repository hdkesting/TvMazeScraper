// <copyright file="QueryRating.cs" company="Hans Keﬆing">
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
    using Microsoft.WindowsAzure.Storage.Table;
    using Newtonsoft.Json;
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
        /// <param name="req">The req.</param>
        /// <param name="cacheTableService">The cache table service.</param>
        /// <param name="queueService">The queue service.</param>
        /// <param name="log">The log.</param>
        /// <returns>A rating, if found.</returns>
        [FunctionName("QueryRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req,
            CacheTableService cacheTableService,
            QueueService queueService,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string imdbId = req.Query["imdbid"];

            if (string.IsNullOrWhiteSpace(imdbId))
            {
                return new BadRequestResult();
            }

            var rating = await cacheTableService.GetRating(imdbId).ConfigureAwait(false);

            if (rating is null)
            {
            }
            else
            {
                // there is a rating (might be "unknown")
                log.LogInformation($"Got a rating of {rating.Rating} from {rating.RetrievalDate}.");

                if (!cacheTableService.IsRecentEnough(rating.RetrievalDate))
                {
                    log.LogInformation("Rating was old, so try and get fresh one.");
                    await queueService.QueueMessage(new RatingRequest { ImdbId = imdbId }).ConfigureAwait(false);
                }
            }

            decimal? ratingValue = rating?.Rating;

            if (ratingValue.HasValue)
            {
                return new OkObjectResult(ratingValue.Value);
            }
            else
            {
                return new NoContentResult();
            }
        }
    }
}
