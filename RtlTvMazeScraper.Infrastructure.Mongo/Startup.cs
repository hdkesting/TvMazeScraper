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
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        internal static string ConnectionString { get; private set; }

        /// <summary>
        /// Configures this project.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public static void Configure(string connectionString)
        {
            ConnectionString = connectionString;
        }

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
