// <copyright file="HomeController.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.UI.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RtlTvMazeScraper.Core.Interfaces;

    /// <summary>
    /// The default controller.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller"/>
    public class HomeController : Controller
    {
        private readonly IShowService showService;
        private readonly ILogger<HomeController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController" /> class.
        /// </summary>
        /// <param name="showService">The show service.</param>
        /// <param name="logger">The logger.</param>
        public HomeController(
            IShowService showService,
            ILogger<HomeController> logger)
        {
            this.showService = showService;
            this.logger = logger;
        }

        /// <summary>
        /// The default action, showing the start page.
        /// </summary>
        /// <returns>A view.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var counts = await this.showService.GetCounts().ConfigureAwait(false);

            // argument binding is by position, not name
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1&tabs=aspnetcore2x#log-message-template
            this.logger.LogDebug("Got counts of {ShowCount} shows and {MemberCount} castmembers", counts.ShowCount, counts.MemberCount);

            return this.View(counts);
        }
    }
}