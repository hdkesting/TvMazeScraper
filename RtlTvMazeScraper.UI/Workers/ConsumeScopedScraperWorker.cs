// <copyright file="ConsumeScopedScraperWorker.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.UI.Workers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class ConsumeScopedScraperWorker : IHostedService
    {
        private readonly IServiceProvider services;
        private readonly ILogger<ConsumeScopedScraperWorker> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumeScopedScraperWorker"/> class.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="logger">The logger.</param>
        public ConsumeScopedScraperWorker(
            IServiceProvider services,
            ILogger<ConsumeScopedScraperWorker> logger)
        {
            this.services = services;
            this.logger = logger;
        }

        public int ShowId { get; internal set; }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns>A completed Task.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation(
                 "Consume Scoped Service Hosted Service is starting.");

            this.DoWork();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        /// <returns>A completed Task.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation(
                  "Consume Scoped Service Hosted Service is stopping.");

            return Task.CompletedTask;
        }

        private void DoWork()
        {
            this.logger.LogInformation(
                "Consume Scoped Service Hosted Service is working.");

            using (var scope = this.services.CreateScope())
            {
                var scopedScraperWorker =
                    scope.ServiceProvider
                        .GetRequiredService<IScraperWorker>();

                scopedScraperWorker.AddShowIds(this.ShowId, 20);
                scopedScraperWorker.DoWork();
            }
        }
    }
}
