// <copyright file="IncomingRatingQueueRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Infrastructure.Repositories.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Newtonsoft.Json.Linq;
    using TvMazeScraper.Core.Interfaces;
    using TvMazeScraper.Core.Transfer;

    /// <summary>
    /// Repository to access the Azure Queue that stores the ratings.
    /// </summary>
    /// <seealso cref="TvMazeScraper.Core.Interfaces.IIncomingRatingRepository" />
    [Obsolete("replace by a call to QueryRating", true)]
    public class IncomingRatingQueueRepository : IIncomingRatingRepository
    {
        private readonly CloudQueue queue;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncomingRatingQueueRepository"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="InvalidOperationException">The connection string 'imdbstorage' is missing.</exception>
        public IncomingRatingQueueRepository(IConfiguration configuration)
        {
            string connStr = configuration?.GetConnectionString("imdbstorage");
            if (connStr is null)
            {
                throw new InvalidOperationException("The connection string 'imdbstorage' is missing.");
            }

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connStr);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            this.queue = queueClient.GetQueueReference("myqueue");
        }

        /// <summary>
        /// Gets the queued ratings.
        /// </summary>
        /// <param name="maxCount">The maximum number of messages to read.</param>
        /// <returns>
        /// A Task of (possibly empty) list of ratings.
        /// </returns>
        public async Task<List<ShowRating>> GetQueuedRatings(int maxCount)
        {
            await this.queue.CreateIfNotExistsAsync().ConfigureAwait(false);

            var list = await this.queue.GetMessagesAsync(maxCount).ConfigureAwait(false);

            var ratings = new List<ShowRating>();

            foreach (var message in list)
            {
                var jsonString = message.AsString;

                // var json = $"{{ \"showid\": {showId}, \"rating\": {rating} }}";
                dynamic json = JObject.Parse(jsonString);

                ratings.Add(new ShowRating { ShowId = json.showid, Rating = json.rating });

                // it is processed, so should be removed
                await this.queue.DeleteMessageAsync(message).ConfigureAwait(false);
            }

            return ratings;
        }
    }
}
