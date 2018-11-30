// <copyright file="IShowRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Core.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TvMazeScraper.Core.DTO;
    using TvMazeScraper.Core.Transfer;

    /// <summary>
    /// Repository to read shows.
    /// </summary>
    public interface IShowRepository
    {
        /// <summary>
        /// Gets the list of shows.
        /// </summary>
        /// <param name="startId">The id to start at.</param>
        /// <param name="count">The number of shows to download.</param>
        /// <returns>A list of shows.</returns>
        Task<List<ShowDto>> GetShows(int startId, int count);

        /// <summary>
        /// Gets the counts of shows and cast.
        /// </summary>
        /// <returns>A tuple having counts of shows and castmembers.</returns>
        Task<StorageCount> GetCounts();

        /// <summary>
        /// Gets the shows including cast.
        /// </summary>
        /// <param name="page">The page number (0-based).</param>
        /// <param name="pagesize">The size of the page.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        Task<List<ShowDto>> GetShowsWithCast(int page, int pagesize, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the maximum show identifier.
        /// </summary>
        /// <returns>The highest ID.</returns>
        Task<int> GetMaxShowId();

        /// <summary>
        /// Stores the show list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="getCastOfShow">A function to get the cast of one show.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        Task StoreShowList(List<ShowDto> list, Func<int, Task<List<CastMemberDto>>> getCastOfShow);

        /// <summary>
        /// Gets the show by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>One Show (if found).</returns>
        Task<ShowDto> GetShowById(int id);

        /// <summary>
        /// Sets the rating of the specified show.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <param name="rating">The rating.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        Task SetRating(int showId, decimal rating);

        /// <summary>
        /// Gets <paramref name="count" /> shows without rating.
        /// </summary>
        /// <param name="count">The max number of shows to return.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        Task<List<ShowDto>> GetShowsWithoutRating(int count);

        /// <summary>
        /// Gets the oldest shows based on last modified date.
        /// </summary>
        /// <param name="count">The max count.</param>
        /// <returns>A list of shows.</returns>
        Task<List<ShowDto>> GetOldestShows(int count);
    }
}
