// <copyright file="IApiRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Core.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TvMazeScraper.Core.Support.Events;
    using TvMazeScraper.Core.Transfer;

    /// <summary>
    /// Repository to access an API.
    /// </summary>
    public interface IApiRepository
    {
        /// <summary>
        /// Requests the json from the specified URL, for TV Maze.
        /// </summary>
        /// <param name="relativePath">The relative URL.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation. Defaults to <see cref="CancellationToken.None"/>.</param>
        /// <returns>
        /// The response status and the json (if any).
        /// </returns>
        Task<ApiResponse> RequestJsonForTvMaze(Uri relativePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Requests the json from the specified URL, for OMDb.
        /// </summary>
        /// <param name="relativePath">The relative URL.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation. Defaults to <see cref="CancellationToken.None"/>.</param>
        /// <returns>
        /// The response status and the json (if any).
        /// </returns>
        Task<ApiResponse> RequestJsonForOmdb(Uri relativePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts the enriching of the show data.
        /// </summary>
        /// <remarks>
        /// It may take a while (hours, days) for the answer to arrive.
        /// </remarks>
        /// <param name="message">The message.</param>
        /// <returns>A Task with a value indicating whether the operation was succesful.</returns>
        Task<bool> StartEnrichingShow(ShowStoredEvent message);
    }
}
