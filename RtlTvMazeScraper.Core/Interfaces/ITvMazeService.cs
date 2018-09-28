// <copyright file="ITvMazeService.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Core.Interfaces
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TvMazeScraper.Core.DTO;
    using TvMazeScraper.Core.Transfer;

    /// <summary>
    /// A service to read from TV Maze.
    /// </summary>
    public interface ITvMazeService
    {
        /// <summary>
        /// Scrapes the shows by their initial.
        /// </summary>
        /// <param name="searchWord">The word to search for.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        Task<List<ShowDto>> ScrapeShowsBySearch(string searchWord, CancellationToken cancellationToken = default);

        /// <summary>
        /// Scrapes the cast members for a particular show.
        /// </summary>
        /// <param name="showid">The show's id.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A list of cast members.
        /// </returns>
        Task<List<CastMemberDto>> ScrapeCastMembers(int showid, CancellationToken cancellationToken = default);

        /// <summary>
        /// Scrapes a batch of shows by their identifier, starting from the supplied <paramref name="start"/>.
        /// </summary>
        /// <param name="start">The start ID.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A tuple: number of shows tried, list of shows found.
        /// </returns>
        Task<ScrapeBatchResult> ScrapeBatchById(int start, CancellationToken cancellationToken = default);

        /// <summary>
        /// Scrapes the single show by its identifier.
        /// </summary>
        /// <param name="showId">The show's identifier.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A Task with Show and status code.</returns>
        Task<ScrapeResult> ScrapeSingleShowById(int showId, CancellationToken cancellationToken = default);
    }
}
