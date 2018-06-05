﻿// <copyright file="SettingRepository.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Repositories
{
    using System.Configuration;
    using RtlTvMazeScraper.Interfaces;

    /// <summary>
    /// A repository for settings.
    /// </summary>
    /// <seealso cref="RtlTvMazeScraper.Interfaces.ISettingRepository" />
    public class SettingRepository : ISettingRepository
    {
        private string connstr;

        /// <summary>
        /// Gets the connection string to the local database.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString => this.connstr ?? (this.connstr = ConfigurationManager.ConnectionStrings["ShowContext"].ConnectionString);

        /// <summary>
        /// Gets the url for TV Maze.
        /// </summary>
        /// <value>
        /// The tv maze host.
        /// </value>
        public string TvMazeHost => ConfigurationManager.AppSettings["tvmaze"];
    }
}