// <copyright file="ShowService.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Model;
    using RtlTvMazeScraper.Core.Transfer;

    /// <summary>
    /// Service to access shows.
    /// </summary>
    [CLSCompliant(false)]
    public class ShowService : IShowService
    {
        private readonly IShowRepository showRepository;
        private readonly ILogger<ShowService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowService" /> class.
        /// </summary>
        /// <param name="showRepository">The show repository.</param>
        /// <param name="logger">The logger.</param>
        public ShowService(IShowRepository showRepository, ILogger<ShowService> logger)
        {
            this.showRepository = showRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the list of shows.
        /// </summary>
        /// <param name="startId">The (show-)id to start at.</param>
        /// <param name="count">The number of shows to download.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        public async Task<List<Show>> GetShows(int startId, int count)
        {
            try
            {
                return await this.showRepository.GetShows(startId, count).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failure to get {count} shows from #{startId} onwards.", count, startId);
                return new List<Show>();
            }
        }

        /// <summary>
        /// Gets the shows including cast.
        /// </summary>
        /// <param name="page">The page number (0-based).</param>
        /// <param name="pagesize">The size of the page.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        public async Task<List<Show>> GetShowsWithCast(int page, int pagesize, CancellationToken cancellationToken)
        {
            try
            {
                return await this.showRepository.GetShowsWithCast(page, pagesize, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failure to get page {pagenr} of shows.", page);
                return new List<Show>();
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
        public async Task StoreShowList(List<Show> list, Func<int, Task<List<CastMember>>> getCastOfShow)
        {
            try
            {
                await this.showRepository.StoreShowList(list, getCastOfShow).ConfigureAwait(false);
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
        public async Task<Show> GetShowById(int id)
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
    }
}
