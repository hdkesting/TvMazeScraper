using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RtlTvMazeScraper.Interfaces;
using RtlTvMazeScraper.Models;

namespace RtlTvMazeScraper.Services
{
    public class TvMazeService: ITvMazeService
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

        public async Task<List<CastMember>> ScrapeCastMembers(int showid)
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
                        };

                        var bd = person["birthday"];
                        if (bd.Type == JTokenType.Date)
                        {
                            member.Birthdate = (DateTime?)bd;
                        }

                        result.Add(member);
                    }

                    return result;
                }

                // pause for retry
                await Task.Delay(delay);
                delay = delay.Add(delay);
            }
        }

        public async Task<(int count, List<Show> shows)> ScrapeById(int start)
        {
            const int MAX = 40;
            int count = 0;

            var list = new List<Show>();

            while (count < MAX)
            {
                var (status, json) = await this.PerformRequest($"http://api.tvmaze.com/shows/{(start + count)}?embed=cast");

                if (status == (HttpStatusCode)429)
                {
                    // too much, so back off
                    break;
                }

                if (status == HttpStatusCode.OK)
                {
                    var jshow = JObject.Parse(json);
                    var show = new Show
                    {
                        Id = (int)jshow["id"],
                        Name = (string)jshow["name"]
                    };

                    var jcast = (JArray)jshow["_embedded"]["cast"];
                    foreach (var container in jcast)
                    {
                        var person = (JObject)container["person"];
                        var member = new CastMember
                        {
                            Id = (int)person["id"],
                            Name = (string)person["name"],
                        };

                        var bd = person["birthday"];
                        if (bd.Type == JTokenType.Date)
                        {
                            member.Birthdate = (DateTime?)bd;
                        }

                        show.Cast.Add(member);
                    }

                    list.Add(show);
                }

                count++;
            }

            return (count, list);
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