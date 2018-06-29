// <copyright file="TvMazeCastResultNames.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Support
{
    /// <summary>
    /// Attribute names in JSON result of a query to TvMaze for the cast of a particular show.
    /// </summary>
    internal static class TvMazeCastResultNames
    {
        /// <summary>
        /// The name of the attribute holding the person's information.
        /// </summary>
        public const string PersonContainer = "person";

        /// <summary>
        /// The attribute name of the person identifier within the person object.
        /// </summary>
        public const string PersonId = "id";

        /// <summary>
        /// The attribute name of the person's name within the person object.
        /// </summary>
        public const string PersonName = "name";

        /// <summary>
        /// The attribute name of the person's birthday within the person object.
        /// </summary>
        public const string PersonBirthday = "birthday";
    }
}
