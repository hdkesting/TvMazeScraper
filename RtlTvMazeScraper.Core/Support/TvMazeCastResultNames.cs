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
#pragma warning disable CA1802 // Use literals where appropriate
        /// <summary>
        /// The name of the attribute holding the person's information.
        /// </summary>
        public static readonly string PersonContainer = "person";

        /// <summary>
        /// The attribute name of the person identifier within the person object.
        /// </summary>
        public static readonly string PersonId = "id";

        /// <summary>
        /// The attribute name of the person's name within the person object.
        /// </summary>
        public static readonly string PersonName = "name";

        /// <summary>
        /// The attribute name of the person's birthday within the person object.
        /// </summary>
        public static readonly string PersonBirthday = "birthday";
#pragma warning restore CA1802 // Use literals where appropriate
    }
}
