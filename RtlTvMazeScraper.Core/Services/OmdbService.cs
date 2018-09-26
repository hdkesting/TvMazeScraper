// <copyright file="OmdbService.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Services
{
    using System;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Support;
    using RtlTvMazeScraper.Core.Support.Events;

    /// <summary>
    /// The service that calls the Open Movie Database API.
    /// </summary>
    /// <seealso cref="RtlTvMazeScraper.Core.Interfaces.IOmdbService" />
    /// <seealso cref="System.IDisposable" />
    public sealed class OmdbService : IOmdbService
    {
        private readonly IShowService showService;
        private readonly IApiRepository apiRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="OmdbService" /> class.
        /// </summary>
        /// <param name="showService">The show service.</param>
        /// <param name="apiRepository">The API repository.</param>
        public OmdbService(IShowService showService, IApiRepository apiRepository)
        {
            this.showService = showService;
            this.apiRepository = apiRepository;
        }

        /// <summary>
        /// Gets or sets the OMDb API key.
        /// </summary>
        /// <value>
        /// The API key.
        /// </value>
        public static string ApiKey { get; set; }

        /// <summary>
        /// Enriches the show, by getting the IMDb rating.
        /// </summary>
        /// <param name="message">The message detailing the show to enrich.</param>
        /// <returns>
        /// A <see cref="Task" />.
        /// </returns>
        public async Task EnrichShowWithRating(ShowStoredEvent message)
        {
            var uri = new Uri($"?apikey={ApiKey}&i={message.ImdbId}", UriKind.Relative);
            var response = await this.apiRepository.RequestJsonForOmdb(uri).ConfigureAwait(false);

            if (response.Status != System.Net.HttpStatusCode.OK)
            {
                return;
            }

            dynamic json = JObject.Parse(response.Json);

            decimal rating = json.imdbRating;

            await this.showService.SetRating(message.ShowId, rating).ConfigureAwait(false);
        }
    }
}
