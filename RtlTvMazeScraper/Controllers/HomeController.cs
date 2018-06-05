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
        private readonly IShowRepository showRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="showRepository">The show repository.</param>
        public HomeController(IShowRepository showRepository)
        {
            this.showRepository = showRepository;
        }

        /// <summary>
        /// The default action.
        /// </summary>
        /// <returns>A view.</returns>
        public async Task<ActionResult> Index()
        {
            var (shows, members) = await this.showRepository.GetCounts();

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
            this.ViewBag.Message = "Your application description page.";

            return this.View();
        }

        /// <summary>
        /// Shows the (dummy) "Contact" page.
        /// </summary>
        /// <returns>A View.</returns>
        public ActionResult Contact()
        {
            this.ViewBag.Message = "Your contact page.";

            return this.View();
        }
    }
}