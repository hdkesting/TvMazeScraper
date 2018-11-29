// <copyright file="RatingHostedService.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.UI.Workers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using TvMazeScraper.Core.Interfaces;

    /// <summary>
    /// Hosted service to add ratings to stored shows.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.IHostedService" />
    /// <seealso cref="System.IDisposable" />
    public sealed class RatingHostedService : IHostedService, IDisposable
    {
        private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(6);

        private readonly IServiceProvider services;
        private readonly ILogger<RatingHostedService> logger;
        private Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingHostedService"/> class.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="logger">The logger.</param>
        public RatingHostedService(
            IServiceProvider services,
            ILogger<RatingHostedService> logger)
        {
            this.services = services;
            this.logger = logger;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns>A completed Task.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Timed Background Service is starting.");

            // schedule to repeat indefinitely
            this.timer = new Timer(
                this.DoWork,
                null,
                TimeSpan.FromSeconds(1), // slight delay before first activation
                CheckInterval);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        /// <returns>A completed Task.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Timed Background Service is stopping.");

            this.timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.timer?.Dispose();
        }

        /// <summary>
        /// Does the work by reading and processing the queue of ratings.
        /// </summary>
        /// <param name="state">The state.</param>
        private async void DoWork(object state)
        {
            using (var scope = this.services.CreateScope())
            {
                ////var ratingProcessor =
                ////    scope.ServiceProvider
                ////        .GetRequiredService<IIncomingRatingProcessor>();

                ////await ratingProcessor.ProcessIncomingRatings().ConfigureAwait(false);

                /* show service: get number of shows without rating
                 * loop through list, trying to get rating (service that calls Infrastructure.Remote.RatingQueryRepository)
                 * if found, set (using show service)
                 */

                var showService = scope.ServiceProvider.GetRequiredService<IShowService>();

                var shows = await showService.GetShowsWithoutRating(20).ConfigureAwait(false);

                // TODO process them
            }
        }
    }
}
