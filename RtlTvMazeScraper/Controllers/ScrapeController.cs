using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using RtlTvMazeScraper.Interfaces;
using RtlTvMazeScraper.Services;

namespace RtlTvMazeScraper.Controllers
{
    public class ScrapeController : Controller
    {
        private readonly IShowRepository showRepository;
        private readonly ITvMazeService tvMazeService;

        public ScrapeController(
            IShowRepository showRepository,
            ITvMazeService tvMazeService)
        {
            this.showRepository = showRepository;
            this.tvMazeService = tvMazeService;
        }

        // GET: Scrape
        public async Task<ActionResult> Index()
        {
            var max = await showRepository.GetMaxShowId();
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
                var list = await tvMazeService.ScrapeShowsByInitial(initial);

                await showRepository.StoreShowList(list, id => tvMazeService.ScrapeCastMembers(id));

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
            const string key = "noresult";
            if (start < 1) start = 1;

            var (count, list) = await tvMazeService.ScrapeById(start);

            if (list.Any())
            {
                await showRepository.StoreShowList(list, null);
                TempData.Remove(key);
            }
            else
            {
                int failcount = 0;
                if (TempData.ContainsKey(key))
                {
                    failcount = (int)TempData[key];
                    if (failcount > 3)
                    {
                        // apparently no more shows to load
                        return RedirectToAction("Index", "Home");
                    }
                }

                failcount++;
                TempData[key] = failcount;
            }

            ViewBag.Start = start + count;
            return View();
        }
    }
}