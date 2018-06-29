// <copyright file="SettingRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Infrastructure.Repositories.Local
{
    using System;
    using RtlTvMazeScraper.Core.Interfaces;

    /// <summary>
    /// A repository for settings.
    /// </summary>
    /// <seealso cref="ISettingRepository" />
    [CLSCompliant(false)]
    public class SettingRepository : ISettingRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingRepository"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public SettingRepository(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var cfgSection = configuration?.GetSection("Config");
            if (cfgSection == null)
            {
                throw new InvalidOperationException("The config section 'Config' is missing.");
            }

            this.TvMazeHost = cfgSection["tvmaze"];

            if (this.TvMazeHost == null)
            {
                throw new InvalidOperationException("The 'Config' section is missing a value for 'tvmaze'.");
            }
        }

        /// <summary>
        /// Gets the url for TV Maze.
        /// </summary>
        /// <value>
        /// The tv maze host.
        /// </value>
        public string TvMazeHost { get; }
    }
}