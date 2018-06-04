using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RtlTvMazeScraper.Models;

namespace RtlTvMazeScraper.Interfaces
{
    public interface ITvMazeService
    {
        Task<List<Show>> ScrapeShowsByInitial(string initial);

        Task<List<CastMember>> ScrapeCastMembers(int showid);

        Task<(int count, List<Show> shows)> ScrapeById(int start);
    }
}
