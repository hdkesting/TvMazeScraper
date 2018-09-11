// <copyright file="Startup.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using RtlTvMazeScraper.Core.Interfaces;
using RtlTvMazeScraper.Infrastructure.Sql.Interfaces;
using RtlTvMazeScraper.Infrastructure.Sql.Model;
using RtlTvMazeScraper.Infrastructure.Sql.Repositories;

namespace RtlTvMazeScraper.Infrastructure.Sql
{
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
            services.AddScoped<IShowContext, ShowContext>();
        }
    }
}
