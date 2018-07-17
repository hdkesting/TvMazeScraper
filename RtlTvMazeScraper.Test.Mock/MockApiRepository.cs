// <copyright file="MockApiRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Test.Mock
{
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using RtlTvMazeScraper.Core.Transfer;

    /// <summary>
    /// Mock version of TvMaze API repository, reading from a Stream (usually an embedded resource).
    /// </summary>
    /// <seealso cref="RtlTvMazeScraper.Core.Interfaces.IApiRepository" />
    public class MockApiRepository : Core.Interfaces.IApiRepository
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
            if (contentStream == null)
            {
                throw new System.ArgumentNullException(nameof(contentStream));
            }

            using (var sr = new StreamReader(contentStream))
            {
                this.contentToReturn = sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Requests the json from the content that was set.
        /// </summary>
        /// <param name="relativePath">The relative path (ignored).</param>
        /// <param name="cancellation">The cancellation token (not used).</param>
        /// <returns>
        /// The response status and the json (if any).
        /// </returns>
        public Task<ApiResponse> RequestJson(string relativePath, CancellationToken cancellation = default(CancellationToken))
        {
            return Task.FromResult(new ApiResponse(this.StatusToReturn, this.contentToReturn));
        }
    }
}
