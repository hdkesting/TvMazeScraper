// <copyright file="Startup.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Infrastructure.Mongo
{
    using Microsoft.Extensions.DependencyInjection;
    using RtlTvMazeScraper.Core.Interfaces;

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
            services.AddTransient<IShowRepository, Repositories.MongoShowRepository>(); // or singleton?
        }
    }
}
