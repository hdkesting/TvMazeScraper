using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using RtlTvMazeScraper.Models;

namespace RtlTvMazeScraper.Services
{
    public class TvMazeService
    {
        public async Task<List<Show>> ScrapeShowsByInitial(string initial)
        {
            var delay = TimeSpan.FromSeconds(5);
            while (true)
            {
                var (status, json) = await this.PerformRequest("http://api.tvmaze.com/search/shows?q=" + initial);

                if (status != (HttpStatusCode)429)
                {
                    if (status != HttpStatusCode.OK)
                    {
                        return null;
                    }

                    var result = new List<Show>();

                    // read json
                    var array = JArray.Parse(json);
                    foreach (var showcontainer in array)
                    {
                        var jshow = (JObject)showcontainer["show"];
                        var show = new Show()
                        {
                            Id = (int)jshow["id"],
                            Name = (string)jshow["name"]
                        };

                        result.Add(show);
                    }

                    return result;
                }

                // pause for retry
                await Task.Delay(delay);
                delay = delay.Add(delay);
            }
        }

        internal async Task<List<CastMember>> ScrapeCastMembers(int showid)
        {
            var delay = TimeSpan.FromSeconds(5);
            while (true)
            {
                var (status, json) = await this.PerformRequest($"http://api.tvmaze.com/shows/{showid}/cast");

                if (status != (HttpStatusCode)429)
                {
                    if (status != HttpStatusCode.OK)
                    {
                        return null;
                    }

                    var result = new List<CastMember>();

                    // read json
                    var array = JArray.Parse(json);
                    foreach (var role in array)
                    {
                        var person = (JObject)role["person"];
                        var member = new CastMember()
                        {
                            Id = (int)person["id"],
                            Name = (string)person["name"],
                            Birthdate = (DateTime?)person["birthday"]
                        };

                        result.Add(member);
                    }

                    return result;
                }

                // pause for retry
                await Task.Delay(delay);
                delay = delay.Add(delay);
            }
        }

        private async Task<(HttpStatusCode status, string json)> PerformRequest(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var text = await response.Content.ReadAsStringAsync();
                    return (response.StatusCode, text);
                }
                else
                {
                    return (response.StatusCode, "");
                }
            }
        }
    }
}