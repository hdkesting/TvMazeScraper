// <copyright file="MockApiRepository.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Test.Mock
{
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    public class MockApiRepository : Interfaces.IApiRepository
    {
        private string contentToReturn;

        /// <summary>
        /// Gets or sets the status to return.
        /// </summary>
        /// <value>
        /// The status to return.
        /// </value>
        public HttpStatusCode StatusToReturn { get; set; } = HttpStatusCode.OK;

        /// <summary>
        /// Reads the content to return from the supplied stream.
        /// </summary>
        /// <remarks>
        /// Use Assembly.GetManifestResourceStream to read an embedded resource.
        /// </remarks>
        /// <param name="contentStream">The content stream.</param>
        public void ReadContent(Stream contentStream)
        {
            using (var sr = new StreamReader(contentStream))
            {
                this.contentToReturn = sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Requests the json from the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="retryOnBusy">if set to <c>true</c>, retry on a 429 result after a progressive delay (not applicable in test environment).</param>
        /// <returns>
        /// The response status and the json (if any).
        /// </returns>
        public Task<(HttpStatusCode status, string json)> RequestJson(string url, bool retryOnBusy)
        {
            return Task.FromResult((this.StatusToReturn, this.contentToReturn));
        }
    }
}
