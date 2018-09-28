// <copyright file="TimedHostedService.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.UI.Workers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A timed service to trigger loading shows.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.IHostedService" />
    /// <seealso cref="System.IDisposable" />
    public sealed class TimedHostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider services;
        private readonly ILogger<TimedHostedService> logger;
        private Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedHostedService"/> class.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="logger">The logger.</param>
        public TimedHostedService(
            IServiceProvider services,
            ILogger<TimedHostedService> logger)
        {
            this.services = services;
            this.logger = logger;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.timer?.Dispose();
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns>A completed Task.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Timed Background Service is starting.");

            // schedule once
            this.timer = new Timer(
                this.DoWork,
                null,
                TimeSpan.FromSeconds(1),
                Timeout.InfiniteTimeSpan);

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

        private async void DoWork(object state)
        {
            this.logger.LogInformation("Timed Background Service is working.");

            using (var scope = this.services.CreateScope())
            {
                var scopedScraperWorker =
                    scope.ServiceProvider
                        .GetRequiredService<IScraperWorker>();

                var res = await scopedScraperWorker.DoWorkOnManyShows().ConfigureAwait(false);

                // schedule again, depending on result of worker.DoWork
                TimeSpan delay = TimeSpan.FromMilliseconds(50);
                switch (res)
                {
                    case WorkResult.Busy:
                        delay = TimeSpan.FromSeconds(30);
                        break;

                    case WorkResult.Empty:
                        delay = TimeSpan.FromSeconds(20);
                        break;

                    case WorkResult.Error:
                        delay = TimeSpan.FromSeconds(60);
                        break;
                }

                this.timer.Change(
                    delay,
                    Timeout.InfiniteTimeSpan);
            }

            this.logger.LogInformation("Timed Background Service was working.");
        }
    }
}
