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
    using RtlTvMazeScraper.UI.ViewModels;

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
        /// Tests the default action (Index view).
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task TestDefaultAction()
        {
            // arrange
            this.context.Shows.Add(new Show { Id = 42, Name = "HitchHikers Guide to the Galaxy" });
            this.context.Shows.Add(new Show { Id = 12, Name = "Some other show" });
            this.context.CastMembers.Add(new CastMember { MemberId = 1, ShowId = 42, Name = "Ford Prefect", Birthdate = new DateTime(1500, 1, 1) });
            this.context.CastMembers.Add(new CastMember { MemberId = 2, ShowId = 42, Name = "Arthur Dent", Birthdate = new DateTime(1960, 12, 31) });
            this.context.CastMembers.Add(new CastMember { MemberId = 5, ShowId = 12, Name = "Someone", Birthdate = new DateTime(1980, 12, 31) });
            this.context.SaveChanges();

            // note that the tvmazeservice and logger are not used in this action
            var controller = new ScrapeController(showService: this.showService, tvMazeService: null, logger: null);

            // act
            var response = await controller.Index().ConfigureAwait(false);

            // assert
            response.Should().BeAssignableTo<ViewResult>(because: "this should always return a View.");

            var viewResponse = (ViewResult)response;
            viewResponse.Model.Should().BeAssignableTo<int>();
            viewResponse.Model.Should().Be(42, because: "that was the highest ID specified.");

            this.showRepoLogger.WarnCount.Should().Be(0);
            this.showRepoLogger.ErrorCount.Should().Be(0);
        }

        /// <summary>
        /// Tests the scrape by initial action, without parameter.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task TestScrapeAlpha_WithoutParam()
        {
            // arrange
            var scrapeLogger = new DebugLogger<ScrapeController>();
            var controller = new ScrapeController(showService: this.showService, tvMazeService: null, logger: scrapeLogger);

            // act
            var response = await controller.ScrapeAlpha().ConfigureAwait(false);

            // assert
            response.Should().BeAssignableTo<ViewResult>(because: "this should now return a View.");
            var viewResponse = (ViewResult)response;
            viewResponse.Model.Should().BeAssignableTo<ScrapeAlphaViewModel>();

            var model = (ScrapeAlphaViewModel)viewResponse.Model;
            model.PreviousInitial.Should().BeNullOrEmpty();
            model.NextInitial.Should().Be("A");
        }

        /// <summary>
        /// Tests the scrape by initial action, with "A" as parameter.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task TestScrapeAlpha_WithAParam()
        {
            // arrange
            ISettingRepository settingRepo = new MockSettingsRepository();

            var apiRepo = new MockApiRepository();
            apiRepo.ReadContent(typeof(MockApiRepository).Assembly.GetManifestResourceStream(typeof(MockApiRepository), "Data.TvMazeSearchByA.json"));
            var apiSvclogger = new DebugLogger<TvMazeService>();

            var tvmazeService = new TvMazeService(apiRepo, apiSvclogger);

            var scrapeCtrlLogger = new DebugLogger<ScrapeController>();
            var controller = new ScrapeController(showService: this.showService, tvMazeService: tvmazeService, logger: scrapeCtrlLogger);

            // act
            var response = await controller.ScrapeAlpha("A").ConfigureAwait(false);

            // assert
            response.Should().BeAssignableTo<ViewResult>(because: "this should now return a View.");
            var viewResponse = (ViewResult)response;
            viewResponse.Model.Should().BeAssignableTo<ScrapeAlphaViewModel>();

            var model = (ScrapeAlphaViewModel)viewResponse.Model;
            model.PreviousInitial.Should().Be("A");
            model.NextInitial.Should().Be("B");
            model.PreviousCount.Should().NotBe(0);
        }

        /// <summary>
        /// Tests the scrape by initial action, with "Z" as parameter.
        /// </summary>
        /// <returns>A Task.</returns>
        [TestMethod]
        public async Task TestScrapeAlpha_WithZParam()
        {
            // arrange
            ISettingRepository settingRepo = new MockSettingsRepository();

            var apiRepo = new MockApiRepository();
            apiRepo.ReadContent(typeof(MockApiRepository).Assembly.GetManifestResourceStream(typeof(MockApiRepository), "Data.TvMazeSearchByA.json"));
            var apiSvclogger = new DebugLogger<TvMazeService>();

            var tvmazeService = new TvMazeService(apiRepo, apiSvclogger);

            var scrapeCtrlLogger = new DebugLogger<ScrapeController>();
            var controller = new ScrapeController(showService: this.showService, tvMazeService: tvmazeService, logger: scrapeCtrlLogger);

            // act
            var response = await controller.ScrapeAlpha("Z").ConfigureAwait(false);

            // assert
            response.Should().BeAssignableTo<RedirectToActionResult>(because: "this should now redirect to Home.");
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
