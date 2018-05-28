using System.Threading.Tasks;
using System.Web.Mvc;
using RtlTvMazeScraper.Repositories;

namespace RtlTvMazeScraper.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var repo = new ShowRepository();

            var (shows, members) = await repo.GetCounts();

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