// <copyright file="IApiRepository.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Interfaces
{
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Repository to access an API.
    /// </summary>
    public interface IApiRepository
    {
        /// <summary>
        /// Requests the json from the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="retryOnBusy">if set to <c>true</c>, retry on a 429 result after a progressive delay.</param>
        /// <returns>The response status and the json (if any).</returns>
        Task<(HttpStatusCode status, string json)> RequestJson(string url, bool retryOnBusy);
    }
}
