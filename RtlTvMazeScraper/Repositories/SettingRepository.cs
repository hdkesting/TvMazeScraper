using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RtlTvMazeScraper.Repositories
{
    public class SettingRepository
    {
        private readonly string connstr;

        public SettingRepository()
        {
            this.connstr = ConfigurationManager.ConnectionStrings["ShowContext"].ConnectionString;
        }

    }
}