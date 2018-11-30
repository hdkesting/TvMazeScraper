// <copyright file="RatingService.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TvMazeScraper.Core.DTO;
    using TvMazeScraper.Core.Interfaces;

    /// <summary>
    /// Enrich shows with IMDB rating.
    /// </summary>
    /// <seealso cref="TvMazeScraper.Core.Interfaces.IRatingService" />
    public class RatingService : IRatingService
    {
        private readonly IShowRepository showRepository;
        private readonly IRatingRequestRepository ratingRequestRepository;
        private readonly ILogger<RatingService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RatingService"/> class.
        /// </summary>
        /// <param name="showRepository">The show repository.</param>
        /// <param name="ratingRequestRepository">The rating request repository.</param>
        /// <param name="logger">The logger.</param>
        public RatingService(
            IShowRepository showRepository,
            IRatingRequestRepository ratingRequestRepository,
            ILogger<RatingService> logger)
        {
            this.showRepository = showRepository;
            this.ratingRequestRepository = ratingRequestRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Processes a batch of ratings.
        /// </summary>
        /// <param name="count">The number of ratings to try.</param>
        /// <returns>A Task.</returns>
        public async Task ProcessSomeRatings(int count)
        {
            var shows = await this.GetShowsForRating(count).ConfigureAwait(false);

            foreach (var show in shows)
            {
                var rating = await this.ratingRequestRepository.RequestRating(show.ImdbId).ConfigureAwait(false);
                if (rating.HasValue)
                {
                    await this.showRepository.SetRating(show.Id, rating.Value).ConfigureAwait(false);
                }
            }
        }

        private async Task<IList<ShowDto>> GetShowsForRating(int count)
        {
            // first, try and get a batch that doesn't have a rating yet
            var list = await this.showRepository.GetShowsWithoutRating(count).ConfigureAwait(false);

            if (list.Any())
            {
                return list;
            }

            // then, just get the oldest ratings and try and get a more recent rating
            list = await this.showRepository.GetOldestShows(count).ConfigureAwait(false);
            return list;
        }
    }
}
