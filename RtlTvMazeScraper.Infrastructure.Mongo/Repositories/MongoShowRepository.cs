// <copyright file="MongoShowRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Infrastructure.Mongo.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;
    using TvMazeScraper.Core.DTO;
    using TvMazeScraper.Core.Interfaces;
    using TvMazeScraper.Core.Transfer;
    using TvMazeScraper.Infrastructure.Mongo.Model;

#pragma warning disable CA1812 // Avoid uninstantiated internal classes, because it is instantiated through DI.

    /// <summary>
    /// Repository for shows, using MongoDB as back-end.
    /// </summary>
    /// <seealso cref="TvMazeScraper.Core.Interfaces.IShowRepository" />
    internal class MongoShowRepository : IShowRepository
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {
        private const string DatabaseName = "tvmaze";
        private const string CollectionName = "shows";

        private readonly MongoClient client;
        private readonly IMongoCollection<ShowWithCast> collection;
        private readonly ILogger<MongoShowRepository> logger;

        // ref: http://mongodb.github.io/mongo-csharp-driver/2.7/getting_started/quick_tour/

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoShowRepository" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public MongoShowRepository(ILogger<MongoShowRepository> logger)
        {
            this.client = new MongoClient(Startup.ConnectionString);

            var database = this.client.GetDatabase(DatabaseName);
            this.collection = database.GetCollection<ShowWithCast>(CollectionName);
            this.logger = logger;
        }

        /// <summary>
        /// Gets the counts of shows and cast.
        /// </summary>
        /// <returns>
        /// A tuple having counts of shows and castmembers.
        /// </returns>
        public async Task<StorageCount> GetCounts()
        {
            var showcount = await this.collection.CountDocumentsAsync(show => true).ConfigureAwait(false);

            // Attempt at counting the cast. May load the entire list in memory?
            var castcount = this.collection.AsQueryable().SelectMany(s => s.Cast).Distinct().Count();

            return new StorageCount { MemberCount = castcount, ShowCount = (int)showcount };
        }

        /// <summary>
        /// Gets the maximum show identifier.
        /// </summary>
        /// <returns>
        /// The highest ID.
        /// </returns>
        public async Task<int> GetMaxShowId()
        {
            var sorter = Builders<ShowWithCast>.Sort.Descending(s => s.Id);

            var lastshow = await this.collection.Find(s => true).Sort(sorter).FirstOrDefaultAsync().ConfigureAwait(false);

            return lastshow?.Id ?? 0;
        }

        /// <summary>
        /// Gets the show by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// One Show (if found).
        /// </returns>
        public async Task<ShowDto> GetShowById(int id)
        {
            var filter = Builders<ShowWithCast>.Filter.Eq(s => s.Id, id);
            var show = await this.collection.Find(filter).FirstOrDefaultAsync().ConfigureAwait(false);

            return ConvertShowToDto(show);
        }

        /// <summary>
        /// Gets the list of shows including cast.
        /// </summary>
        /// <param name="startId">The id to start at.</param>
        /// <param name="count">The (max) number of shows to download.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        public async Task<List<ShowDto>> GetShows(int startId, int count)
        {
            var filterBuilder = Builders<ShowWithCast>.Filter;
            var filter = filterBuilder.Gte(s => s.Id, startId) & filterBuilder.Lt(s => s.Id, startId + count);

            var shows = await this.collection.Find(filter).ToListAsync().ConfigureAwait(false);

            return shows.Select(ConvertShowToDto).ToList();
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
        public Task<List<ShowDto>> GetShowsWithCast(int page, int pagesize, CancellationToken cancellationToken)
        {
            return this.GetShows(page * pagesize, pagesize);
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
            var mongoinsertlist = new List<ShowWithCast>(list.Count);
            var mongoupdatelist = new List<ShowWithCast>(list.Count);

            foreach (var orgshow in list)
            {
                var filter = Builders<ShowWithCast>.Filter.Eq(s => s.Id, orgshow.Id);
                var storedShow = await this.collection.Find(filter).FirstOrDefaultAsync().ConfigureAwait(false);

                if (storedShow == null)
                {
                    var show = new ShowWithCast
                    {
                        Id = orgshow.Id,
                        Name = orgshow.Name,
                        ImdbId = orgshow.ImdbId,
                        ImdbRating = orgshow.ImdbRating,
                    };

                    await ReplaceCast(orgshow, show).ConfigureAwait(false);

                    mongoinsertlist.Add(show);
                }
                else
                {
                    storedShow.Name = orgshow.Name;
                    storedShow.ImdbId = orgshow.ImdbId;
                    storedShow.ImdbRating = orgshow.ImdbRating;

                    await ReplaceCast(orgshow, storedShow).ConfigureAwait(false);

                    await this.collection.ReplaceOneAsync(filter, storedShow).ConfigureAwait(false);
                }
            }

            // then store the shows including their cast
            if (mongoinsertlist.Any())
            {
                await this.collection.InsertManyAsync(mongoinsertlist).ConfigureAwait(false);
            }

            async Task ReplaceCast(ShowDto orgshow, ShowWithCast show)
            {
                if (!(orgshow.CastMembers is null) && orgshow.CastMembers.Any())
                {
                    show.Cast.Clear();
                    show.Cast.AddRange(orgshow.CastMembers);
                }
                else if (!(getCastOfShow is null))
                {
                    var cast = await getCastOfShow(show.Id).ConfigureAwait(false);
                    show.Cast = cast;
                }
            }
        }

        /// <summary>
        /// Sets the rating of the specified show.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <param name="rating">The rating.</param>
        /// <returns>
        /// A <see cref="Task" />.
        /// </returns>
        public async Task SetRating(int showId, decimal rating)
        {
            var filter = Builders<ShowWithCast>.Filter.Eq(s => s.Id, showId);
            var show = await this.collection.Find(filter).FirstOrDefaultAsync().ConfigureAwait(false);
            if (show != null)
            {
                show.ImdbRating = rating;
                await this.collection.ReplaceOneAsync(filter, show).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets <paramref name="count" /> shows without rating.
        /// </summary>
        /// <param name="count">The max number of shows to return.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        public async Task<List<ShowDto>> GetShowsWithoutRating(int count)
        {
            var filter = Builders<ShowWithCast>.Filter.Eq(s => s.ImdbRating, null);
            var shows = await this.collection.Find(filter)
                .SortBy(s => s.Id)
                .Limit(count)
                .ToListAsync().ConfigureAwait(false);

            // TODO also require imdb id

            return shows.Select(ConvertShowToDto).ToList();
        }

        /// <summary>
        /// Gets the oldest shows based on last modified date.
        /// </summary>
        /// <param name="count">The max count.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        public async Task<List<ShowDto>> GetOldestShows(int count)
        {
            var shows = await this.collection.Find(null)
                .SortBy(s => s.LastModified)
                .Limit(count)
                .ToListAsync().ConfigureAwait(false);

            // TODO also require imdb id

            return shows.Select(ConvertShowToDto).ToList();
        }

        private static ShowDto ConvertShowToDto(ShowWithCast mongoShow)
        {
            // remove the n:m relation
            var show = new ShowDto
            {
                Id = mongoShow.Id,
                Name = mongoShow.Name,
                ImdbId = mongoShow.ImdbId,
                ImdbRating = mongoShow.ImdbRating,
            };
            show.CastMembers.AddRange(mongoShow.Cast);

            return show;
        }
    }
}
