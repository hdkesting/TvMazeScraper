// <copyright file="TvMazeServiceTest.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Test
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Services;

    /// <summary>
    /// Tests the <see cref="TvMazeService"/> against mock data.
    /// </summary>
    [TestClass]
    public class TvMazeServiceTest
    {
        /// <summary>
        /// Tests the search by initial.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task TestSearchByInitial()
        {
            ISettingRepository settingRepo = new Mock.MockSettingsRepository();

            var apiRepo = new Mock.MockApiRepository();
            apiRepo.ReadContent(this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "Mock.Data.TvMazeSearchByA.json"));
            var svclogger = new Mock.DebugLogger<TvMazeService>();

            var svc = new TvMazeService(settingRepo, apiRepo, svclogger);
            var result = await svc.ScrapeShowsBySearch("a").ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Count.Should().Be(10, because: "there are 10 shows in the sample.");
            result.Where(s => s.CastMembers.Any()).Count().Should().Be(0, because: "there is no cast in this show data.");
            result.Select(m => m.Id).Distinct().Count().Should().Be(10, because: "ID is unique");
        }

        /// <summary>
        /// Tests the read of the show's cast.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task TestReadCast()
        {
            ISettingRepository settingRepo = new Mock.MockSettingsRepository();
            var apiRepo = new Mock.MockApiRepository();
            apiRepo.ReadContent(this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "Mock.Data.TvMazeATeamCast.json"));
            var svclogger = new Mock.DebugLogger<TvMazeService>();

            var svc = new TvMazeService(settingRepo, apiRepo, svclogger);
            var result = await svc.ScrapeCastMembers(1058).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Count.Should().Be(7, because: "there are 7 members in the sample.");
            result.Where(m => m.Birthdate.HasValue).Count().Should().Be(6, because: "I set one birthdate to null (as happens).");
            result.Where(m => m.MemberId <= 0).Any().Should().Be(false, because: "everyone has a positive id number.");
            result.Select(m => m.MemberId).Distinct().Count().Should().Be(7, because: "ID is unique");
            result.Where(m => m.Name == null).Any().Should().Be(false, because: "everyone has a name defined.");
        }

        /// <summary>
        /// Tests the read show and cast with server overload.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task TestReadShowAndCast_WithOverload()
        {
            ISettingRepository settingRepo = new Mock.MockSettingsRepository();
            var svclogger = new Mock.DebugLogger<TvMazeService>();
            var apiRepo = new Mock.MockApiRepository
            {
                StatusToReturn = Core.Support.Constants.ServerTooBusy,
            };

            var svc = new TvMazeService(settingRepo, apiRepo, svclogger);
            var (count, shows) = await svc.ScrapeById(999_999).ConfigureAwait(false);

            count.Should().Be(0);
        }

        /// <summary>
        /// Tests the read show and cast when nothing is found.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task TestReadShowAndCast_NothingFound()
        {
            ISettingRepository settingRepo = new Mock.MockSettingsRepository();
            var svclogger = new Mock.DebugLogger<TvMazeService>();
            var apiRepo = new Mock.MockApiRepository
            {
                StatusToReturn = System.Net.HttpStatusCode.NotFound,
            };

            var svc = new TvMazeService(settingRepo, apiRepo, svclogger)
            {
                MaxNumberOfShowsToScrape = 10,
            };

            var (count, shows) = await svc.ScrapeById(999_999).ConfigureAwait(false);

            count.Should().Be(10, because: "I set 10 as the number of tries.");
            shows.Count.Should().Be(0, because: "nothing was supposed to be found.");
        }

        /// <summary>
        /// Tests the read show and cast with success.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task TestReadShowAndCast_Success()
        {
            ISettingRepository settingRepo = new Mock.MockSettingsRepository();
            var svclogger = new Mock.DebugLogger<TvMazeService>();
            var apiRepo = new Mock.MockApiRepository();
            apiRepo.ReadContent(this.GetType().Assembly.GetManifestResourceStream(this.GetType(), "Mock.Data.TvMazeATeamWithCast.json"));

            var svc = new TvMazeService(settingRepo, apiRepo, svclogger)
            {
                MaxNumberOfShowsToScrape = 1,
            };
            var (count, shows) = await svc.ScrapeById(1058).ConfigureAwait(false);

            shows.Count.Should().Be(1);
            shows[0].CastMembers.Count.Should().Be(7);
            shows[0].CastMembers.Where(m => m.Birthdate.HasValue).Count().Should().Be(6);
        }
    }
}
