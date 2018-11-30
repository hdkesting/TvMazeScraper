// <copyright file="RatingRequestRepository.cs" company="Hans Keﬆing">
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

    /// <summary>
    /// Request ratings from the service.
    /// </summary>
    /// <seealso cref="IRatingRequestRepository" />
    public class RatingRequestRepository : IRatingRequestRepository
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<RatingRequestRepository> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingRequestRepository"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <param name="logger">The logger.</param>
        public RatingRequestRepository(
            IHttpClientFactory httpClientFactory,
            ILogger<RatingRequestRepository> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        /// <summary>
        /// Requests the rating for the specified IMDB identifier.
        /// </summary>
        /// <param name="imdbId">The imdb identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The rating or <c>null</c>.
        /// </returns>
        public async Task<decimal?> RequestRating(string imdbId, CancellationToken cancellationToken = default)
        {
            this.logger.LogDebug("Trying to get a rating for {ImdbId}", imdbId);
            using (var httpClient = this.httpClientFactory.CreateClient(TvMazeScraper.Core.Support.Constants.OmdbMicroService))
            {
                var uri = new Uri("api/QueryRating?imdbid=" + imdbId, UriKind.Relative);
                var response = await httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var text = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    this.logger.LogInformation("Found rating {Rating} for {ImdbId}", text, imdbId);

                    if (decimal.TryParse(text, out decimal value))
                    {
                        return value;
                    }
                }

                // no response, error or invalid number
                this.logger.LogInformation("Got status {Status} for {ImdbId}", response.StatusCode, imdbId);
                return null;
            }
        }
    }
}
