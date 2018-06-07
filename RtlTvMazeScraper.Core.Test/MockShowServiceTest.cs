// <copyright file="MockShowServiceTest.cs" company="Hans Kesting">
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

    [TestClass]
    public class MockShowServiceTest
    {
        private ShowService showService;
        private Mock.MockShowContext mockContext;

        [TestInitialize]
        public void Initialize()
        {
            var settingsRepo = new SettingRepository();
            var logRepo = new LogDebugRepository();
            this.mockContext = new Mock.MockShowContext();
            var showRepo = new ShowRepository(settingsRepo, logRepo, this.mockContext);
            this.showService = new ShowService(showRepo, logRepo);
        }

        [TestMethod]
        public async Task TestItemCount()
        {
            // arrange
            this.mockContext.Shows.Add(new Model.Show { Id = 42, Name = "HitchHikers Guid To The Galaxy" });
            this.mockContext.Shows.Add(new Model.Show { Id = 12, Name = "Some other show" });
            this.mockContext.CastMembers.Add(new Model.CastMember { MemberId = 1, ShowId = 42, Name = "Ford Prefect", Birthdate = new DateTime(1500, 1, 1) });
            this.mockContext.CastMembers.Add(new Model.CastMember { MemberId = 2, ShowId = 42, Name = "Arthur Dent", Birthdate = new DateTime(1960, 12, 31) });
            this.mockContext.CastMembers.Add(new Model.CastMember { MemberId = 5, ShowId = 12, Name = "Someone", Birthdate = new DateTime(1980, 12, 31) });

            // act
            var (shows, members) = await this.showService.GetCounts();

            // assert
            shows.Should().Be(2, because: "I added 2.");
            members.Should().Be(3, because: "I added 3.");
        }
    }
}
