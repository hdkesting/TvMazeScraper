using System.Threading.Tasks;
using System.Web.Mvc;
using RtlTvMazeScraper.Interfaces;
using RtlTvMazeScraper.Repositories;

namespace RtlTvMazeScraper.Controllers
{
    public class HomeController : Controller
    {
        private readonly IShowRepository showRepository;

        public HomeController(IShowRepository showRepository)
        {
            this.showRepository = showRepository;
        }

        public async Task<ActionResult> Index()
        {
            var (shows, members) = await showRepository.GetCounts();

            ViewBag.Shows = shows;
            ViewBag.Members = members;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}