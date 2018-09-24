// <copyright file="LiveShowServiceTest.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Test
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RtlTvMazeScraper.Core.Services;
    using RtlTvMazeScraper.Infrastructure.Sql.Model;
    using RtlTvMazeScraper.Infrastructure.Sql.Repositories;
    using RtlTvMazeScraper.Test.Mock;

    /// <summary>
    /// Test the <see cref="ShowService"/> against a live (local) database.
    /// </summary>
    /// <remarks>
    /// The service just passes the requests through to the repository, so when I mock that repository, there is nothing left to test. 
    /// Some features may work differently on a live system compared to a mock, such as parallel queries.
    /// </remarks>
    [TestClass]
#if !DEBUG
    [Ignore]
#endif
    public sealed class LiveShowServiceIntegrationTest
    {
        private ShowService showService;
        private DebugLogger<ShowService> showServiceLogger;

        /// <summary>
        /// Initializes this test instance.
        /// </summary>
        /// <remarks>
        /// NB: I assume the application has run for some time, so that the database does contain these records.
        /// </remarks>
        [TestInitialize]
        public void Initialize()
        {
            // var connstr = ConfigurationManager.ConnectionStrings["ShowConnection"].ConnectionString;
            var connstr = @"Server=.\sqlexpress;Database=tvmaze;Trusted_Connection=True;Connection Timeout=30;Application Name=TvMazeScraperTest";
            var options = new DbContextOptionsBuilder<ShowContext>()
                                 .UseSqlServer(connstr)
                                 .Options;
            var context = new ShowContext(options);

            var repologger = new DebugLogger<ShowRepository>();
            var showRepo = new ShowRepository(repologger, context);

            this.showServiceLogger = new DebugLogger<ShowService>();
            this.showService = new ShowService(showRepo, this.showServiceLogger);
        }

        /// <summary>
        /// Tests the live item count.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task TestLiveItemCount()
        {
            var counts = await this.showService.GetCounts().ConfigureAwait(false);

            this.showServiceLogger.ErrorCount.Should().Be(0, because: "I don't want errors.");
            counts.ShowCount.Should().BeGreaterThan(0, because: "I expect hundreds.");
            counts.MemberCount.Should().BeGreaterThan(0, because: "I expect multiple per show.");
        }

        /// <summary>
        /// Gets the shows with cast.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task GetShowsWithCast()
        {
            var shows = await this.showService.GetShowsWithCast(0, 10, CancellationToken.None).ConfigureAwait(false);

            shows.Should().NotBeNull();
            shows.Count.Should().BeLessOrEqualTo(10, because: "I requested a page of size 10.");

            shows.Where(s => s.CastMembers.Any()).Count().Should().BeGreaterThan(0, because: "I expect at least some to have a cast defined.");
        }

        /// <summary>
        /// Gets the shows with cast, checking paging.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task GetShowsWithCast_CheckPaging()
        {
            var shows0 = await this.showService.GetShowsWithCast(0, 10, CancellationToken.None).ConfigureAwait(false);
            var shows3 = await this.showService.GetShowsWithCast(3, 10, CancellationToken.None).ConfigureAwait(false);

            shows0.Should().NotBeNull();
            shows0.Count.Should().BeGreaterThan(0, because: "I expect enough stored shows.");
            shows0.Count.Should().BeLessOrEqualTo(10, because: "I requested a page of size 10.");

            shows3.Should().NotBeNull();
            shows3.Count.Should().BeGreaterThan(0, because: "I expect enough stored shows.");
            shows3.Count.Should().BeLessOrEqualTo(10, because: "I requested a page of size 10.");

            shows3.Where(s3 => shows0.Any(s0 => s0.Id == s3.Id)).Count().Should().Be(0, because: "Ï requested a *different* page");
        }

        /// <summary>
        /// Gets the maximum show identifier.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task GetMaxShowId()
        {
            var max = await this.showService.GetMaxShowId().ConfigureAwait(false);

            max.Should().BeGreaterThan(1000, because: "there are at least this many shows stored.");
        }

        /// <summary>
        /// Gets the single show.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task GetSingleShow()
        {
            var show = await this.showService.GetShowById(42).ConfigureAwait(false);

            show.Should().NotBeNull();
            show.Id.Should().BeGreaterThan(0);
            show.CastMembers.Count.Should().BeGreaterThan(0);
        }
    }
}
