// <copyright file="ITvMazeService.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using RtlTvMazeScraper.Core.Models;

    /// <summary>
    /// A service to read from TV Maze.
    /// </summary>
    public interface ITvMazeService
    {
        /// <summary>
        /// Scrapes the shows by their initial.
        /// </summary>
        /// <param name="initial">The initial.</param>
        /// <returns>A list of shows.</returns>
        Task<List<Show>> ScrapeShowsByInitial(string initial);

        /// <summary>
        /// Scrapes the cast members for a particular show.
        /// </summary>
        /// <param name="showid">The showid.</param>
        /// <returns>A list of cast members.</returns>
        Task<List<CastMember>> ScrapeCastMembers(int showid);

        /// <summary>
        /// Scrapes shows by their identifier.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns>A tuple: number of shows tried, list of shows found.</returns>
        Task<(int count, List<Show> shows)> ScrapeById(int start);
    }
}
