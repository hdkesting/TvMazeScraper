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
    using RtlTvMazeScraper.Core.Support;
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
        /// Requests the json from the specified URL, for TV Maze.
        /// </summary>
        /// <param name="relativePath">The relative URL.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation. Default <see cref="CancellationToken.None"/>.</param>
        /// <returns>
        /// The response status and the json (if any).
        /// </returns>
        public Task<ApiResponse> RequestJsonForTvMaze(Uri relativePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.RequestJson(Constants.TvMazeClientWithRetry, relativePath, cancellationToken);
        }

        /// <summary>
        /// Requests the json from the specified URL, for OMDb.
        /// </summary>
        /// <param name="relativePath">The relative URL.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation. Default <see cref="CancellationToken.None"/>.</param>
        /// <returns>
        /// The response status and the json (if any).
        /// </returns>
        public Task<ApiResponse> RequestJsonForOmdb(Uri relativePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.RequestJson(Constants.OmdbClient, relativePath, cancellationToken);
        }

        /// <summary>
        /// Requests the json.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The response status and the json (if any).
        /// </returns>
        private async Task<ApiResponse> RequestJson(string key, Uri relativePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var httpClient = this.httpClientFactory.CreateClient(key))
            {
                var response = await httpClient.GetAsync(relativePath, cancellationToken).ConfigureAwait(false);

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
