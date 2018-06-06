// <copyright file="LiveShowServiceTest.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Test
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RtlTvMazeScraper.Core.Services;
    using RtlTvMazeScraper.Infrastructure.Repositories.Local;

    /// <summary>
    /// Test the <see cref="ShowService"/> against a live (local) database.
    /// </summary>
    /// <remarks>
    /// The service just passes the requests through to the repository, so when I mock that repository, there is nothing left to test.
    /// </remarks>
    [TestClass]
    public class LiveShowServiceTest
    {
        private ShowService showService;

        [TestInitialize]
        public void Initialize()
        {
            var settingsRepo = new SettingRepository();
            var showRepo = new ShowRepository(settingsRepo);
            var logRepo = new LogDebugRepository();
            this.showService = new ShowService(showRepo, logRepo);
        }

        [TestMethod]
        public async Task TestLiveItemCount()
        {
            var (shows, members) = await this.showService.GetCounts();

            shows.Should().BeGreaterThan(0, because: "I expect hundreds.");
            members.Should().BeGreaterThan(0, because: "I expect multiple per show.");
        }

        [TestMethod]
        public async Task GetShowsWithCast()
        {
            var shows = await this.showService.GetShowsWithCast(0, 10);

            shows.Should().NotBeNull();
            shows.Count.Should().BeLessOrEqualTo(10, because: "I requested a page of size 10.");

            shows.Where(s => s.CastMembers.Any()).Count().Should().BeGreaterThan(0, because: "I expect at least some to have a cast defined.");
        }

        [TestMethod]
        public async Task GetShowsWithCast_CheckPaging()
        {
            var shows0 = await this.showService.GetShowsWithCast(0, 10);
            var shows3 = await this.showService.GetShowsWithCast(3, 10);

            shows0.Should().NotBeNull();
            shows0.Count.Should().BeGreaterThan(0, because: "I expect enough stored shows.");
            shows0.Count.Should().BeLessOrEqualTo(10, because: "I requested a page of size 10.");

            shows3.Should().NotBeNull();
            shows3.Count.Should().BeGreaterThan(0, because: "I expect enough stored shows.");
            shows3.Count.Should().BeLessOrEqualTo(10, because: "I requested a page of size 10.");

            shows3.Where(s3 => shows0.Any(s0 => s0.Id == s3.Id)).Count().Should().Be(0, because: "Ï requested a *different* page");
        }

    }
}
