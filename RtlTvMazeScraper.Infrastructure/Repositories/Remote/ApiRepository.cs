// <copyright file="ApiRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Infrastructure.Repositories.Remote
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Transfer;

    /// <summary>
    /// A respository to access a remote webAPI endpoint.
    /// </summary>
    public class ApiRepository : IApiRepository
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiRepository" /> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        public ApiRepository(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Requests the json from the specified URL.
        /// </summary>
        /// <param name="relativePath">The relative URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The response status and the json (if any).
        /// </returns>
        public async Task<ApiResponse> RequestJson(string relativePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            var key = Core.Support.Constants.TvMazeClientWithRetry;

            using (var httpClient = this.httpClientFactory.CreateClient(key))
            {
#pragma warning disable CA2234 // Pass system uri objects instead of strings
                var response = await httpClient.GetAsync(relativePath, cancellationToken).ConfigureAwait(false);
#pragma warning restore CA2234 // Pass system uri objects instead of strings

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return new ApiResponse(HttpStatusCode.OK, text);
                }
                else
                {
                    return new ApiResponse(response.StatusCode, string.Empty);
                }
            }
        }
    }
}
