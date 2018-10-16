// <copyright file="IncomingRatingProcessor.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Core.Workers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using TvMazeScraper.Core.Interfaces;

    /// <summary>
    /// Processes the ratings as written to the Azure queue.
    /// </summary>
    /// <seealso cref="TvMazeScraper.Core.Interfaces.IIncomingRatingProcessor" />
    public class IncomingRatingProcessor : IIncomingRatingProcessor
    {
        private readonly IIncomingRatingRepository repository;
        private readonly IShowService showService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncomingRatingProcessor" /> class.
        /// </summary>
        /// <param name="repository">The repository for incoming ratings.</param>
        /// <param name="showService">The show service.</param>
        public IncomingRatingProcessor(
            IIncomingRatingRepository repository,
            IShowService showService)
        {
            this.repository = repository;
            this.showService = showService;
        }

        /// <summary>
        /// Processes the available ratings.
        /// </summary>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task ProcessIncomingRatings()
        {
            // TODO: read queue (possibly with a limit) and store all ratings found (using ShowService)
            throw new NotImplementedException();
        }
    }
}
