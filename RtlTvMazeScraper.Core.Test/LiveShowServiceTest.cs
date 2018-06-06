// <copyright file="LiveShowServiceTest.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Test
{
    using System;
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
        [TestMethod]
        public async Task TestLiveItemCount()
        {
            var settingsRepo = new SettingRepository();
            var showRepo = new ShowRepository(settingsRepo);
            var showService = new ShowService(showRepo);

            var (shows, members) = await showService.GetCounts();

            shows.Should().BeGreaterThan(0);
            members.Should().BeGreaterThan(0);
        }
    }
}
