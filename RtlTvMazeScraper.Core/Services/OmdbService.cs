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
    public sealed class OmdbService : IOmdbService, IDisposable
    {
        private readonly Guid subscription;
        private readonly MessageHub messageHub;
        private readonly IShowService showService;
        private readonly IApiRepository apiRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="OmdbService" /> class.
        /// </summary>
        /// <param name="messageHub">The message hub.</param>
        /// <param name="showService">The show service.</param>
        /// <param name="apiRepository">The API repository.</param>
        public OmdbService(MessageHub messageHub, IShowService showService, IApiRepository apiRepository)
        {
            this.messageHub = messageHub;
            this.showService = showService;
            this.apiRepository = apiRepository;
            this.subscription = this.messageHub.Subscribe<ShowStoredEvent>(msg => this.EnrichShowWithRating(msg.ShowId, msg.ImdbId));
        }

        /// <summary>
        /// Gets or sets the OMDb API key.
        /// </summary>
        /// <value>
        /// The API key.
        /// </value>
        public static string ApiKey { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.messageHub.Unsubscribe(this.subscription);
        }

        /// <summary>
        /// Enriches the show, by getting the IMDb rating.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <param name="imdbId">The IMDb identifier.</param>
        /// <returns>
        /// A <see cref="Task" />.
        /// </returns>
        public async Task EnrichShowWithRating(int showId, string imdbId)
        {
            var uri = new Uri($"?apikey={ApiKey}&i={imdbId}", UriKind.Relative);
            var response = await this.apiRepository.RequestJsonForOmdb(uri).ConfigureAwait(false);

            if (response.Status != System.Net.HttpStatusCode.OK)
            {
                return;
            }

            dynamic json = JObject.Parse(response.Json);

            decimal rating = json.imdbRating;

            await this.showService.SetRating(showId, rating).ConfigureAwait(false);
        }
    }
}
