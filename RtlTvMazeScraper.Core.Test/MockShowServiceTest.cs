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
            var s1 = new Model.Show { Id = 42, Name = "HitchHikers Guide to the Galaxy" };
            var m = new Model.CastMember { Id = 1, Name = "Ford Prefect", Birthdate = new DateTime(1500, 1, 1) };
            s1.ShowCastMembers.Add(new ShowCastMember { Show = s1, CastMember = m });
            m = new Model.CastMember { Id = 2, Name = "Arthur Dent", Birthdate = new DateTime(1960, 12, 31) };
            s1.ShowCastMembers.Add(new ShowCastMember { Show = s1, CastMember = m });
            this.context.Shows.Add(s1);

            var s2 = new Model.Show { Id = 12, Name = "Some other show" };
            m = new Model.CastMember { Id = 5, Name = "Someone", Birthdate = new DateTime(1980, 12, 31) };
            s2.ShowCastMembers.Add(new ShowCastMember { Show = s2, CastMember = m });
            this.context.Shows.Add(s2);
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
            var s1 = new Model.Show { Id = 42, Name = "HitchHikers Guide to the Galaxy" };
            var m = new Model.CastMember { Id = 1, Name = "Ford Prefect", Birthdate = new DateTime(1500, 1, 1) };
            s1.ShowCastMembers.Add(new ShowCastMember { Show = s1, CastMember = m });
            m = new Model.CastMember { Id = 2, Name = "Arthur Dent", Birthdate = new DateTime(1960, 12, 31) };
            s1.ShowCastMembers.Add(new ShowCastMember { Show = s1, CastMember = m });
            this.context.Shows.Add(s1);

            var s2 = new Model.Show { Id = 12, Name = "Some other show" };
            m = new Model.CastMember { Id = 5, Name = "Someone", Birthdate = new DateTime(1980, 12, 31) };
            s2.ShowCastMembers.Add(new ShowCastMember { Show = s2, CastMember = m });
            this.context.Shows.Add(s2);
            this.context.SaveChanges();

            // act
            var shows = await this.showService.GetShowsWithCast(0, 10, default(CancellationToken)).ConfigureAwait(false);

            // assert
            shows.Should().NotBeNull();
            shows.Count.Should().Be(2, because: "I added 2 shows.");

            shows.Where(s => s.ShowCastMembers.Any()).Count().Should().Be(2, because: "Both shows have cast.");
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
