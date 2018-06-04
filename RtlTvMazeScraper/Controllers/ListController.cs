using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using RtlTvMazeScraper.Interfaces;

namespace RtlTvMazeScraper.Controllers
{
    public class ListController : ApiController
    {
        private readonly IShowRepository showRepository;

        public ListController(IShowRepository showRepository)
        {
            this.showRepository = showRepository;
        }

        [HttpGet]
        [Route("~/list")]
        public async Task<JArray> GetShows(int page = 0, int pagesize = 20)
        {
            if (page < 0) page = 0;
            if (pagesize < 2) pagesize = 2;

            var shows = await this.showRepository.GetShowsWithCast(page, pagesize);

            JArray result = new JArray();

            foreach (var show in shows)
            {

                var cast = new JArray();
                foreach (var member in show.Cast)
                {
                    var cm = new JObject
                    (
                        new JProperty("id", member.Id),
                        new JProperty("name", member.Name),
                        new JProperty("birthday", member.Birthdate)
                    );
                    cast.Add(cm);
                }

                var showObj = new JObject(
                    new JProperty("id", show.Id),
                    new JProperty("name", show.Name),
                    new JProperty("cast", cast));

                result.Add(showObj);
            }

            return result;
        }
    }
}
