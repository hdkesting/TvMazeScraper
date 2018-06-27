// <copyright file="ISettingRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Interfaces
{
    /// <summary>
    /// Repository of settings.
    /// </summary>
    public interface ISettingRepository
    {
        /// <summary>
        /// Gets the url for TV Maze.
        /// </summary>
        /// <value>
        /// The tv maze host.
        /// </value>
        string TvMazeHost { get; }
    }
}