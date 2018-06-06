// <copyright file="TvMazeServiceTest.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>


namespace RtlTvMazeScraper.Core.Test
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Services;

    [TestClass]
    public class TvMazeServiceTest
    {
        [TestMethod]
        public async Task TestSearchByInitial()
        {
            ISettingRepository settingRepo = new Mock.MockSettingsRepository();
            var apiRepo = new Mock.MockApiRepository();
            apiRepo.ReadContent(this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "Mock.Data.TvMazeSearchByA.json"));

            var svc = new TvMazeService(settingRepo, apiRepo);
            var result = await svc.ScrapeShowsBySearch("a");

            result.Should().NotBeNull();
            result.Count.Should().Be(10, because: "there are 10 shows in the sample.");
            result.Where(s => s.Cast.Any()).Count().Should().Be(0, because: "there is no cast in this show data.");
        }

        [TestMethod]
        public async Task TestReadCast()
        {
            ISettingRepository settingRepo = new Mock.MockSettingsRepository();
            var apiRepo = new Mock.MockApiRepository();
            apiRepo.ReadContent(this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "Mock.Data.TvMazeATeamCast.json"));

            var svc = new TvMazeService(settingRepo, apiRepo);
            var result = await svc.ScrapeCastMembers(1058);

            result.Should().NotBeNull();
            result.Count.Should().Be(7, because: "there are 7 members in the sample.");
            result.Where(m => m.Birthdate.HasValue).Count().Should().Be(6, because: "I set one birthdate to null (as happens)");
        }
    }
}
