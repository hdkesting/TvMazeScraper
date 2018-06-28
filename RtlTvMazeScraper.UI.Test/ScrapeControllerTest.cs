// <copyright file="ScrapeControllerTest.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.UI.Test
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Model;
    using RtlTvMazeScraper.Core.Services;
    using RtlTvMazeScraper.Infrastructure.Repositories.Local;
    using RtlTvMazeScraper.Test.Mock;
    using RtlTvMazeScraper.UI.Controllers;

    /// <summary>
    /// Tests for the <see cref="ScrapeController"/>.
    /// </summary>
    [TestClass]
    public sealed class ScrapeControllerTest : IDisposable
    {
        private ShowContext context;
        private ShowService showService;
        private DebugLogger<ShowRepository> showRepoLogger;

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
            this.showRepoLogger = new DebugLogger<ShowRepository>();
            var showRepo = new ShowRepository(this.showRepoLogger, this.context);

            var svclogger = new DebugLogger<ShowService>();
            this.showService = new ShowService(showRepo, svclogger);
        }

        /// <summary>
        /// Cleans up this test instance.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            this.context?.Dispose();
            this.context = null;
        }

        /// <summary>
        /// Tests the default action (Index view).
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task TestDefaultAction()
        {
            // arrange
            ISettingRepository settingRepo = new MockSettingsRepository();

            this.context.Shows.Add(new Show { Id = 42, Name = "HitchHikers Guide to the Galaxy" });
            this.context.Shows.Add(new Show { Id = 12, Name = "Some other show" });
            this.context.CastMembers.Add(new CastMember { MemberId = 1, ShowId = 42, Name = "Ford Prefect", Birthdate = new DateTime(1500, 1, 1) });
            this.context.CastMembers.Add(new CastMember { MemberId = 2, ShowId = 42, Name = "Arthur Dent", Birthdate = new DateTime(1960, 12, 31) });
            this.context.CastMembers.Add(new CastMember { MemberId = 5, ShowId = 12, Name = "Someone", Birthdate = new DateTime(1980, 12, 31) });
            this.context.SaveChanges();

            // note that the tvmazeservice isn't used in this action
            var scrapeLogger = new DebugLogger<ScrapeController>();
            var controller = new ScrapeController(showService: this.showService, tvMazeService: null, logger: null);

            // act
            var response = await controller.Index().ConfigureAwait(false);

            // assert
            response.Should().BeAssignableTo<ViewResult>(because: "this should always return a View.");

            var viewResponse = (ViewResult)response;
            viewResponse.Model.Should().Be(42, because: "that was the highest ID specified.");

            this.showRepoLogger.WarnCount.Should().Be(0);
            this.showRepoLogger.ErrorCount.Should().Be(0);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.context?.Dispose();
            this.context = null;
        }
    }
}
