// <copyright file="QueueService.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.ImdbFunctions.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json;
    using TvMazeScraper.ImdbFunctions.Model;

    /// <summary>
    /// Interacts with the queue of rating requests.
    /// </summary>
    public class QueueService
    {
        private static readonly TimeSpan OmdbDelay = TimeSpan.FromHours(4);
        private readonly CloudQueue queueClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueService"/> class.
        /// </summary>
        /// <param name="queueClient">The incoming queue client.</param>
        public QueueService(
            [Queue("imdbratingqueue", Connection = "imdbrating")]
            CloudQueue queueClient)
        {
            this.queueClient = queueClient;
        }

        /// <summary>
        /// Requeues the message with a long delay.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>A Task.</returns>
        public Task RequeueMessage(string json)
        {
            return this.QueueMessage(json, OmdbDelay);
        }

        /// <summary>
        /// Queues the message.
        /// </summary>
        /// <param name="ratingRequest">The rating request.</param>
        /// <returns>A Task.</returns>
        public Task QueueMessage(RatingRequest ratingRequest)
        {
            var json = JsonConvert.SerializeObject(ratingRequest);
            return this.QueueMessage(json);
        }

        /// <summary>
        /// Deletes the message.
        /// </summary>
        /// <param name="queueItem">The queue item.</param>
        /// <returns>A Task.</returns>
        public Task DeleteMessageAsync(CloudQueueMessage queueItem)
        {
            return this.queueClient.DeleteMessageAsync(queueItem);
        }

        /// <summary>
        /// Adds the message to the queue.
        /// </summary>
        /// <param name="json">The json to store.</param>
        /// <param name="delay">The delay before the message becomes visible (default: <c>null</c> = no delay).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        private async Task QueueMessage(string json, TimeSpan? delay = null)
        {
            var cqm = new CloudQueueMessage(json);
            await this.queueClient.AddMessageAsync(
                message: cqm,
                timeToLive: null,
                initialVisibilityDelay: delay,
                options: null,
                operationContext: null).ConfigureAwait(false);
        }
    }
}
