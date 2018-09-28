// <copyright file="MockSettingsRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Test.Mock
{
    /// <summary>
    /// A mock version of the <see cref="Interfaces.ISettingRepository"/>.
    /// </summary>
    /// <seealso cref="TvMazeScraper.Core.Interfaces.ISettingRepository" />
    public class MockSettingsRepository : Core.Interfaces.ISettingRepository
    {
        /// <summary>
        /// Gets the url for TV Maze.
        /// </summary>
        /// <value>
        /// The tv maze host.
        /// </value>
        public string TvMazeHost => "http://localhost";
    }
}
