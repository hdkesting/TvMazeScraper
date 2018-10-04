// <copyright file="Startup.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Infrastructure.Sql
{
    using Microsoft.Extensions.DependencyInjection;
    using TvMazeScraper.Core.Interfaces;
    using TvMazeScraper.Infrastructure.Sql.Interfaces;
    using TvMazeScraper.Infrastructure.Sql.Model;
    using TvMazeScraper.Infrastructure.Sql.Repositories;

    /// <summary>
    /// Statup as relates to this project.
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Configures the dependecy injection, specific for this project.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void ConfigureDI(IServiceCollection services)
        {
            services.AddTransient<IShowRepository, ShowRepository>();

            // db context
            services.AddTransient<IShowContext, ShowContext>();
        }
    }
}
