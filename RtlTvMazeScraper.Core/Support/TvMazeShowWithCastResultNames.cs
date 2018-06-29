// <copyright file="TvMazeShowWithCastResultNames.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Support
{
    /// <summary>
    /// Attribute names in JSON result of a query to TvMaze for a show including cast.
    /// </summary>
    internal static class TvMazeShowWithCastResultNames
    {
        /// <summary>
        /// The attribute name of the show's identifier.
        /// </summary>
        public const string ShowId = "id";

        /// <summary>
        /// The attribute name of the show's name.
        /// </summary>
        public const string ShowName = "name";

        /// <summary>
        /// The attribute name of the embedded data.
        /// </summary>
        public const string EmbeddedContainer = "_embedded";

        /// <summary>
        /// The attribute name of the cast object within the embedded data.
        /// </summary>
        public const string CastContainer = "cast";

        /// <summary>
        /// The attribute name of the object holding the person's data.
        /// </summary>
        public const string PersonContainer = "person";

        /// <summary>
        /// The attribute name of the person's identifier within the person's data.
        /// </summary>
        /// <remarks>
        /// Even though this constant has the same value as <see cref="ShowId"/>, that should be treated as coincidence. The two should not be merged.
        /// </remarks>
        public const string PersonId = "id";

        /// <summary>
        /// The attribute name of the person's name within the person's data.
        /// </summary>
        public const string PersonName = "name";

        /// <summary>
        /// The attribute name of the person's birthday within the person's data.
        /// </summary>
        public const string PersonBirthday = "birthday";
    }
}
