using System.Threading.Tasks;
using System.Web.Mvc;
using RtlTvMazeScraper.Services;

namespace RtlTvMazeScraper.Controllers
{
    public class ScrapeController : Controller
    {
        private readonly Repositories.ShowRepository showRepo;

        public ScrapeController()
        {
            showRepo = new Repositories.ShowRepository();
        }

        // GET: Scrape
        public async Task<ActionResult> Index()
        {
            var max = await showRepo.GetMaxShowId();
            this.ViewBag.Max = max;

            return View();
        }

        public async Task<ActionResult> ScrapeAlpha(string initial)
        {
            if (string.IsNullOrEmpty(initial))
            {
                initial = "a";
            }
            else
            {
                // perform scrape
                var svc = new TvMazeService();
                var list = await svc.ScrapeShowsByInitial(initial);

                await showRepo.StoreShowList(list, id => svc.ScrapeCastMembers(id));

                if (initial.StartsWith("z"))
                {
                    // done!
                    return RedirectToAction(nameof(Index));
                }

                initial = ((char)(initial[0] + 1)).ToString();
            }

            ViewBag.Initial = initial;

            return View();
        }

        public async Task<ActionResult> Scrape(int start = 1)
        {
            if (start < 1) start = 1;

            var svc = new TvMazeService();

            var (count, list) = await svc.ScrapeById(start);

            await showRepo.StoreShowList(list, null);

            ViewBag.Start = start + count;
            return View();
        }
    }
}