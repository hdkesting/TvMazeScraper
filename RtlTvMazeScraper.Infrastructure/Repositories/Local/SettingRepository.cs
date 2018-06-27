// <copyright file="SettingRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Infrastructure.Repositories.Local
{
    using RtlTvMazeScraper.Core.Interfaces;

    /// <summary>
    /// A repository for settings.
    /// </summary>
    /// <seealso cref="ISettingRepository" />
    public class SettingRepository : ISettingRepository
    {
        /// <summary>
        /// Gets or sets the url for TV Maze.
        /// </summary>
        /// <value>
        /// The tv maze host.
        /// </value>
        public string TvMazeHost { get; set; }
    }
}