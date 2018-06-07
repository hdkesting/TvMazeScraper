// <copyright file="ApiResponse.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Transfer
{
    using System.Net;

    /// <summary>
    /// Response from a (TV Maze) web API call.
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponse"/> class.
        /// </summary>
        public ApiResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponse"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="json">The json.</param>
        public ApiResponse(HttpStatusCode status, string json)
        {
            this.Status = status;
            this.Json = json;
        }

        /// <summary>
        /// Gets or sets the response status of the request.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public HttpStatusCode Status { get; set; }

        /// <summary>
        /// Gets or sets the json result (as string).
        /// </summary>
        /// <value>
        /// The json string.
        /// </value>
        public string Json { get; set; }

        /// <summary>
        /// Deconstructs this instance.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="json">The json.</param>
        public void Deconstruct(out HttpStatusCode status, out string json)
        {
            status = this.Status;
            json = this.Json;
        }
    }
}
