using System.Configuration;
using RtlTvMazeScraper.Interfaces;

namespace RtlTvMazeScraper.Repositories
{
    public class SettingRepository : ISettingRepository
    {
        private string connstr;

        public string ConnectionString
        {
            get { return this.connstr ?? (this.connstr = ConfigurationManager.ConnectionStrings["ShowContext"].ConnectionString); }
        }
    }
}