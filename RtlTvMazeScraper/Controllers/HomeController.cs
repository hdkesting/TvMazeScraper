// <copyright file="HomeController.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using RtlTvMazeScraper.Core.Interfaces;

    /// <summary>
    /// The default controller.
    /// </summary>
    /// <seealso cref="System.Web.Mvc.Controller" />
    public class HomeController : Controller
    {
        private readonly IShowService showService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController" /> class.
        /// </summary>
        /// <param name="showService">The show service.</param>
        public HomeController(IShowService showService)
        {
            this.showService = showService;
        }

        /// <summary>
        /// The default action.
        /// </summary>
        /// <returns>A view.</returns>
        public async Task<ActionResult> Index()
        {
            var (shows, members) = await this.showService.GetCounts();

            this.ViewBag.Shows = shows;
            this.ViewBag.Members = members;
            return this.View();
        }

        /// <summary>
        /// Shows the (dummy) "about" page.
        /// </summary>
        /// <returns>A View.</returns>
        public ActionResult About()
        {
            this.ViewBag.Message = "A sample scraper for TV Maze.";

            return this.View();
        }

        /// <summary>
        /// Shows the (dummy) "Contact" page.
        /// </summary>
        /// <returns>A View.</returns>
        public ActionResult Contact()
        {
            this.ViewBag.Message = "hans_kesting@epam.com";

            return this.View();
        }
    }
}