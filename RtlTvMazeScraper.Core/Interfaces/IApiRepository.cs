// <copyright file="IApiRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using RtlTvMazeScraper.Core.Transfer;

    /// <summary>
    /// Repository to access an API.
    /// </summary>
    public interface IApiRepository
    {
        /// <summary>
        /// Requests the json from the specified URL.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="retryOnBusy">if set to <c>true</c>, retry on a 429 result after a progressive delay.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The response status and the json (if any).
        /// </returns>
        Task<ApiResponse> RequestJson(string relativePath, bool retryOnBusy, CancellationToken cancellationToken = default);
    }
}
