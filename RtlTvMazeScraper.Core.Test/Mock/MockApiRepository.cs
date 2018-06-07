// <copyright file="MockApiRepository.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Test.Mock
{
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using RtlTvMazeScraper.Core.Transfer;

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
        /// Requests the json from the content that was set.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="retryOnBusy">Ignored.</param>
        /// <returns>
        /// The response status and the json (if any).
        /// </returns>
        public Task<ApiResponse> RequestJson(string url, bool retryOnBusy)
        {
            return Task.FromResult(new ApiResponse(this.StatusToReturn, this.contentToReturn));
        }
    }
}
