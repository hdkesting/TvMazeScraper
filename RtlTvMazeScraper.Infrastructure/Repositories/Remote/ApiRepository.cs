// <copyright file="ApiRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Infrastructure.Repositories.Remote
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TvMazeScraper.Core.Interfaces;
    using TvMazeScraper.Core.Support;
    using TvMazeScraper.Core.Support.Events;
    using TvMazeScraper.Core.Transfer;

    /// <summary>
    /// A respository to access a remote webAPI endpoint.
    /// </summary>
    public class ApiRepository : IApiRepository
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<ApiRepository> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiRepository" /> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <param name="logger">The logger.</param>
        public ApiRepository(IHttpClientFactory httpClientFactory, ILogger<ApiRepository> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
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
        /// Starts the enriching of the show data.
        /// </summary>
        /// <remarks>
        /// It may take a while (hours, days) for the answer to arrive.
        /// </remarks>
        /// <param name="message">The message.</param>
        /// <returns>A Task with a value indicating whether the operation was succesful.</returns>
        public async Task<bool> StartEnrichingShow(ShowStoredEvent message)
        {
            try
            {
                using (var httpClient = this.httpClientFactory.CreateClient(Constants.OmdbMicroService))
                {
                    var relUri = new Uri($"api/SubmitToQueue?imdbid={message.ImdbId}&showid={message.ShowId}", UriKind.Relative);
                    var response = await httpClient.GetAsync(relUri).ConfigureAwait(false);

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(ex, "Some issue in using client factory");
                return false;
            }
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
