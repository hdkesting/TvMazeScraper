// <copyright file="IncomingRatingProcessor.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Core.Workers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TvMazeScraper.Core.Interfaces;

    /// <summary>
    /// Processes the ratings as written to the Azure queue.
    /// </summary>
    /// <seealso cref="TvMazeScraper.Core.Interfaces.IIncomingRatingProcessor" />
    [Obsolete("replace by a call to QueryRating", true)]
    public class IncomingRatingProcessor : IIncomingRatingProcessor
    {
        private const int RatingsPerTry = 20;

        private readonly IIncomingRatingRepository repository;
        private readonly IShowService showService;
        private readonly ILogger<IncomingRatingProcessor> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncomingRatingProcessor" /> class.
        /// </summary>
        /// <param name="repository">The repository for incoming ratings.</param>
        /// <param name="showService">The show service.</param>
        /// <param name="logger">The logger.</param>
        public IncomingRatingProcessor(
            IIncomingRatingRepository repository,
            IShowService showService,
            ILogger<IncomingRatingProcessor> logger)
        {
            this.repository = repository;
            this.showService = showService;
            this.logger = logger;
        }

        /// <summary>
        /// Processes the available ratings.
        /// </summary>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task ProcessIncomingRatings()
        {
            // read queue and store all ratings found (using ShowService)
            var ratings = await this.repository.GetQueuedRatings(RatingsPerTry)
                .ConfigureAwait(false);
            this.logger.LogInformation("Found {count} fresh ratings", ratings.Count);

            foreach (var rating in ratings)
            {
                await this.showService.SetRating(rating.ShowId, rating.Rating).ConfigureAwait(false);
            }

            this.logger.LogInformation("Updated {count} fresh ratings", ratings.Count);
        }
    }
}
