// <copyright file="ISettingRepository.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Interfaces
{
    /// <summary>
    /// Repository of settings.
    /// </summary>
    public interface ISettingRepository
    {
        /// <summary>
        /// Gets the connection string to the local database.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        string ConnectionString { get; }

        /// <summary>
        /// Gets the url for TV Maze.
        /// </summary>
        /// <value>
        /// The tv maze host.
        /// </value>
        string TvMazeHost { get; }
    }
}