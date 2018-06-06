// <copyright file="MockSettingsRepository.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Test.Mock
{
    public class MockSettingsRepository : Interfaces.ISettingRepository
    {
        /// <summary>
        /// Gets the connection string to the local database.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString => string.Empty;

        /// <summary>
        /// Gets the url for TV Maze.
        /// </summary>
        /// <value>
        /// The tv maze host.
        /// </value>
        public string TvMazeHost => "http://localhost";
    }
}
