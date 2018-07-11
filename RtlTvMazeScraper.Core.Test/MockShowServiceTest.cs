// <copyright file="MockShowServiceTest.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Test
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RtlTvMazeScraper.Core.Model;
    using RtlTvMazeScraper.Core.Services;
    using RtlTvMazeScraper.Infrastructure.Repositories.Local;
    using RtlTvMazeScraper.Test.Mock;

    /// <summary>
    /// Tests the <see cref="ShowService"/> against an in-memory database.
    /// </summary>
    [TestClass]
    public sealed class MockShowServiceTest : IDisposable
    {
        private ShowContext context;
        private ShowService showService;

        /// <summary>
        /// Initializes this test instance.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<ShowContext>()
                                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                 .Options;
            this.context = new ShowContext(options);
            var repologger = new DebugLogger<ShowRepository>();
            var showRepo = new ShowRepository(repologger, this.context);

            var svclogger = new DebugLogger<ShowService>();
            this.showService = new ShowService(showRepo, svclogger);
        }

        /// <summary>
        /// Tests the item count.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task TestItemCount()
        {
            // arrange
            this.context.Shows.Add(new Model.Show { Id = 42, Name = "HitchHikers Guide to the Galaxy" });
            this.context.Shows.Add(new Model.Show { Id = 12, Name = "Some other show" });
            this.context.CastMembers.Add(new Model.CastMember { MemberId = 1, ShowId = 42, Name = "Ford Prefect", Birthdate = new DateTime(1500, 1, 1) });
            this.context.CastMembers.Add(new Model.CastMember { MemberId = 2, ShowId = 42, Name = "Arthur Dent", Birthdate = new DateTime(1960, 12, 31) });
            this.context.CastMembers.Add(new Model.CastMember { MemberId = 5, ShowId = 12, Name = "Someone", Birthdate = new DateTime(1980, 12, 31) });
            this.context.SaveChanges();

            // act
            var counts = await this.showService.GetCounts().ConfigureAwait(false);

            // assert
            counts.ShowCount.Should().Be(2, because: "I added 2.");
            counts.MemberCount.Should().Be(3, because: "I added 3.");
        }

        /// <summary>
        /// Gets the shows with cast.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task GetShowsWithCast()
        {
            // arrange
            this.context.Shows.Add(new Model.Show { Id = 42, Name = "HitchHikers Guide to the Galaxy" });
            this.context.Shows.Add(new Model.Show { Id = 12, Name = "Some other show" });
            this.context.CastMembers.Add(new Model.CastMember { MemberId = 1, ShowId = 42, Name = "Ford Prefect", Birthdate = new DateTime(1500, 1, 1) });
            this.context.CastMembers.Add(new Model.CastMember { MemberId = 2, ShowId = 42, Name = "Arthur Dent", Birthdate = new DateTime(1960, 12, 31) });
            this.context.CastMembers.Add(new Model.CastMember { MemberId = 5, ShowId = 12, Name = "Someone", Birthdate = new DateTime(1980, 12, 31) });
            this.context.SaveChanges();

            // act
            var shows = await this.showService.GetShowsWithCast(0, 10, default(CancellationToken)).ConfigureAwait(false);

            // assert
            shows.Should().NotBeNull();
            shows.Count.Should().Be(2, because: "I added 2 shows.");

            shows.Where(s => s.CastMembers.Any()).Count().Should().Be(2, because: "Both shows have cast.");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.context?.Dispose();
        }
    }
}
