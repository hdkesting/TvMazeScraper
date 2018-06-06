// <copyright file="ApiRepository.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Infrastructure.Repositories.Remote
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using RtlTvMazeScraper.Core.Interfaces;

    /// <summary>
    /// A respository to access a remote webAPI endpoint.
    /// </summary>
    public class ApiRepository : IApiRepository
    {
        private const int MaxRetryCount = 5;
        private static readonly TimeSpan DelayIncrease = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Requests the json from the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="retryOnBusy">if set to <c>true</c>, retry on a 429 result after a progressive delay.</param>
        /// <returns>The response status and the json (if any).</returns>
        public async Task<(HttpStatusCode status, string json)> RequestJson(string url, bool retryOnBusy)
        {
            TimeSpan delay = TimeSpan.Zero;
            int retrycount = 0;

            using (var httpClient = new HttpClient())
            {
                while (true)
                {
                    var response = await httpClient.GetAsync(url);

                    if (response.StatusCode == Core.Support.Constants.ServerTooBusy)
                    {
                        if (!retryOnBusy)
                        {
                            return (response.StatusCode, string.Empty);
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var text = await response.Content.ReadAsStringAsync();
                        return (response.StatusCode, text);
                    }
                    else
                    {
                        return (response.StatusCode, string.Empty);
                    }

                    retrycount++;

                    if (retrycount > MaxRetryCount)
                    {
                        // give up after several failed tries
                        return (Core.Support.Constants.ServerTooBusy, null);
                    }

                    delay += DelayIncrease;
                    await Task.Delay(delay);
                }
            }
        }
    }
}
