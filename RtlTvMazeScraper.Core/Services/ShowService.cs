// <copyright file="ShowService.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TvMazeScraper.Core.DTO;
    using TvMazeScraper.Core.Interfaces;
    using TvMazeScraper.Core.Support.Events;
    using TvMazeScraper.Core.Transfer;

    /// <summary>
    /// Service to access shows.
    /// </summary>
    [CLSCompliant(false)]
    public class ShowService : IShowService
    {
        private readonly IShowRepository showRepository;
        private readonly ILogger<ShowService> logger;
        private readonly IMessageHub messageHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowService" /> class.
        /// </summary>
        /// <param name="showRepository">The show repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="messageHub">The message hub.</param>
        public ShowService(IShowRepository showRepository, ILogger<ShowService> logger, IMessageHub messageHub)
        {
            this.showRepository = showRepository;
            this.logger = logger;
            this.messageHub = messageHub;
        }

        /// <summary>
        /// Gets the list of shows.
        /// </summary>
        /// <param name="startId">The (show-)id to start at.</param>
        /// <param name="count">The number of shows to download.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        public async Task<List<ShowDto>> GetShows(int startId, int count)
        {
            try
            {
                return await this.showRepository.GetShows(startId, count).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failure to get {count} shows from #{startId} onwards.", count, startId);
                return new List<ShowDto>();
            }
        }

        /// <summary>
        /// Gets the shows including cast.
        /// </summary>
        /// <param name="page">The page number (0-based).</param>
        /// <param name="pagesize">The size of the page.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        public async Task<List<ShowDto>> GetShowsWithCast(int page, int pagesize, CancellationToken cancellationToken)
        {
            try
            {
                return await this.showRepository.GetShowsWithCast(page, pagesize, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failure to get page {pagenr} of shows.", page);
                return new List<ShowDto>();
            }
        }

        /// <summary>
        /// Gets the counts of shows and cast.
        /// </summary>
        /// <returns>
        /// A tuple having counts of shows and castmembers.
        /// </returns>
        public async Task<StorageCount> GetCounts()
        {
            try
            {
                return await this.showRepository.GetCounts().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failure to get counts.");
                return new StorageCount();
            }
        }

        /// <summary>
        /// Gets the maximum show identifier.
        /// </summary>
        /// <returns>
        /// The highest ID.
        /// </returns>
        public async Task<int> GetMaxShowId()
        {
            try
            {
                return await this.showRepository.GetMaxShowId().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failure to get max id.", ex);
                return -1;
            }
        }

        /// <summary>
        /// Stores the show list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="getCastOfShow">A function to get the cast of one show.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task StoreShowList(List<ShowDto> list, Func<int, Task<List<CastMemberDto>>> getCastOfShow)
        {
            try
            {
                await this.showRepository.StoreShowList(list, getCastOfShow).ConfigureAwait(false);

                foreach (var show in list.Where(s => !string.IsNullOrEmpty(s.ImdbId)))
                {
                    await this.messageHub.Publish(new ShowStoredEvent(show.Id, show.ImdbId)).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failure to store shows.");
            }
        }

        /// <summary>
        /// Gets the show by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>One Show (if found).</returns>
        public async Task<ShowDto> GetShowById(int id)
        {
            try
            {
                return await this.showRepository.GetShowById(id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Couldn't get show #{id}.", id);
                return null;
            }
        }

        /// <summary>
        /// Sets the IMDb rating of the specified show.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <param name="rating">The IMDb rating.</param>
        /// <returns>
        /// A <see cref="Task" />.
        /// </returns>
        public Task SetRating(int showId, decimal rating)
        {
            return this.showRepository.SetRating(showId, rating);
        }

        /// <summary>
        /// Gets <paramref name="count" /> shows without rating.
        /// </summary>
        /// <param name="count">The max number of shows to return.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        public Task<List<ShowDto>> GetShowsWithoutRating(int count)
        {
            return this.showRepository.GetShowsWithoutRating(count);
        }
    }
}
