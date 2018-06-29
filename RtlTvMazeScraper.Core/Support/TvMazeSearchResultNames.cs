// <copyright file="TvMazeSearchResultNames.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Support
{
    /// <summary>
    /// Attribute names in JSON result of a search query to TvMaze.
    /// </summary>
    internal static class TvMazeSearchResultNames
    {
        /// <summary>
        /// The attribute name of the show object.
        /// </summary>
        public const string ShowContainer = "show";

        /// <summary>
        /// The attribute name of the show identifier within the show object.
        /// </summary>
        public const string ShowId = "id";

        /// <summary>
        /// The attribute name of the show's name within the show object.
        /// </summary>
        public const string ShowName = "name";
    }
}
