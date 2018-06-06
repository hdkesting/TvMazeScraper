// <copyright file="TvMazeServiceTest.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>


namespace RtlTvMazeScraper.Core.Test
{
    using System;
    using System.Linq;
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
            result.Where(s => s.CastMembers.Any()).Count().Should().Be(0, because: "there is no cast in this show data.");
            result.Select(m => m.Id).Distinct().Count().Should().Be(10, because: "ID is unique");
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
            result.Where(m => m.Birthdate.HasValue).Count().Should().Be(6, because: "I set one birthdate to null (as happens).");
            result.Where(m => m.MemberId <= 0).Any().Should().Be(false, because: "everyone has a positive id number.");
            result.Select(m => m.MemberId).Distinct().Count().Should().Be(7, because: "ID is unique");
            result.Where(m => m.Name == null).Any().Should().Be(false, because: "everyone has a name defined.");
        }

        [TestMethod]
        public async Task TestReadShowAndCast_WithOverload()
        {
            ISettingRepository settingRepo = new Mock.MockSettingsRepository();
            var apiRepo = new Mock.MockApiRepository
            {
                StatusToReturn = (System.Net.HttpStatusCode)429
            };

            var svc = new TvMazeService(settingRepo, apiRepo);
            var (count, shows) = await svc.ScrapeById(999_999);

            count.Should().Be(0);
        }

        [TestMethod]
        public async Task TestReadShowAndCast_NothingFound()
        {
            ISettingRepository settingRepo = new Mock.MockSettingsRepository();
            var apiRepo = new Mock.MockApiRepository
            {
                StatusToReturn = System.Net.HttpStatusCode.NotFound
            };

            var svc = new TvMazeService(settingRepo, apiRepo)
            {
                MaxNumberOfShowsToScrape = 10
            };

            var (count, shows) = await svc.ScrapeById(999_999);

            count.Should().Be(10, because: "I set 10 as the number of tries.");
            shows.Count.Should().Be(0, because: "nothing was supposed to be found.");
        }

        [TestMethod]
        public async Task TestReadShowAndCast_Success()
        {
            ISettingRepository settingRepo = new Mock.MockSettingsRepository();
            var apiRepo = new Mock.MockApiRepository();
            apiRepo.ReadContent(this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "Mock.Data.TvMazeATeamWithCast.json"));

            var svc = new TvMazeService(settingRepo, apiRepo)
            {
                MaxNumberOfShowsToScrape = 1
            };
            var (count, shows) = await svc.ScrapeById(1058);

            shows.Count.Should().Be(1);
            shows[0].CastMembers.Count.Should().Be(7);
            shows[0].CastMembers.Where(m => m.Birthdate.HasValue).Count().Should().Be(6);
        }
    }
}
