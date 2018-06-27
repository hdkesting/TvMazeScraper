// <copyright file="Constants.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Support
{
    using System.Net;

    /// <summary>
    /// Various constatnts.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The http status: "server too busy" as used by TvMaze.
        /// </summary>
        public const HttpStatusCode ServerTooBusy = (HttpStatusCode)429;
    }
}
