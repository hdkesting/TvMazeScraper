// <copyright file="IShowService.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using RtlTvMazeScraper.Core.Models;

    /// <summary>
    /// Service to access shows.
    /// </summary>
    public interface IShowService
    {
        /// <summary>
        /// Gets the list of shows.
        /// </summary>
        /// <param name="startId">The id to start at.</param>
        /// <param name="count">The number of shows to download.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        Task<List<Show>> GetShows(int startId, int count);

        /// <summary>
        /// Gets the shows including cast.
        /// </summary>
        /// <param name="page">The page number (0-based).</param>
        /// <param name="pagesize">The size of the page.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        Task<List<Show>> GetShowsWithCast(int page, int pagesize);

        /// <summary>
        /// Gets the counts of shows and cast.
        /// </summary>
        /// <returns>
        /// A tuple having counts of shows and castmembers.
        /// </returns>
        Task<(int shows, int members)> GetCounts();

        /// <summary>
        /// Gets the maximum show identifier.
        /// </summary>
        /// <returns>
        /// The highest ID.
        /// </returns>
        Task<int> GetMaxShowId();

        /// <summary>
        /// Stores the show list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="getCastOfShow">A function to get the cast of one show.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task StoreShowList(List<Show> list, Func<int, Task<List<CastMember>>> getCastOfShow);
    }
}
