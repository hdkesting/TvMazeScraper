// <copyright file="MongoShowRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Infrastructure.Mongo.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Model;
    using RtlTvMazeScraper.Core.Transfer;
    using RtlTvMazeScraper.Infrastructure.Mongo.Model;

    /// <summary>
    /// Repository for shows, using MongoDB as back-end.
    /// </summary>
    /// <seealso cref="RtlTvMazeScraper.Core.Interfaces.IShowRepository" />
    public class MongoShowRepository : IShowRepository
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
        public async Task<Show> GetShowById(int id)
        {
            var filter = Builders<ShowWithCast>.Filter.Eq(s => s.Id, id);
            var show = await this.collection.Find(filter).FirstOrDefaultAsync().ConfigureAwait(false);

            return ConvertShow(show);
        }

        /// <summary>
        /// Gets the list of shows including cast.
        /// </summary>
        /// <param name="startId">The id to start at.</param>
        /// <param name="count">The (max) number of shows to download.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        public async Task<List<Show>> GetShows(int startId, int count)
        {
            var filterBuilder = Builders<ShowWithCast>.Filter;
            var filter = filterBuilder.Gte(s => s.Id, startId) & filterBuilder.Lt(s => s.Id, startId + count);

            var shows = await this.collection.Find(filter).ToListAsync().ConfigureAwait(false);

            return shows.Select(ConvertShow).ToList();
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
        public Task<List<Show>> GetShowsWithCast(int page, int pagesize, CancellationToken cancellationToken)
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
        public async Task StoreShowList(List<Show> list, Func<int, Task<List<CastMember>>> getCastOfShow)
        {
            var mongolist = new List<ShowWithCast>(list.Count);

            foreach (var orgshow in list)
            {
                var show = new ShowWithCast
                {
                    Id = orgshow.Id,
                    Name = orgshow.Name,
                };

                if (!(orgshow.CastMembers is null) && orgshow.CastMembers.Any())
                {
                    show.Cast.AddRange(orgshow.CastMembers);
                }
                else if (!(getCastOfShow is null))
                {
                    var cast = await getCastOfShow(show.Id).ConfigureAwait(false);
                    show.Cast = cast;
                }

                mongolist.Add(show);
            }

            // then store the shows including their cast
            await this.collection.InsertManyAsync(mongolist).ConfigureAwait(false);
        }

        private static Show ConvertShow(ShowWithCast mongoShow)
        {
            // remove the n:m relation
            var show = new Show { Id = mongoShow.Id, Name = mongoShow.Name };
            show.CastMembers.AddRange(mongoShow.Cast);

            return show;
        }
    }
}
