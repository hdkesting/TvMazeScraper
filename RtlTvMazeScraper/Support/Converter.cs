// <copyright file="Converter.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Support
{
    using System.Collections.Generic;
    using System.Linq;
    using RtlTvMazeScraper.Core.Model;

    /// <summary>
    /// Converts to JSON-serializable format.
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Converts the specified shows.
        /// </summary>
        /// <param name="shows">The shows.</param>
        /// <returns>A list of <see cref="ShowForJson"/>.</returns>
        public static List<ShowForJson> Convert(IEnumerable<Show> shows)
        {
            return shows
                .OrderBy(s => s.Id)
                .Select(s => new ShowForJson
                {
                    Id = s.Id,
                    Name = s.Name,
                    Cast = Convert(s.CastMembers),
                })
                .ToList();
        }

        /// <summary>
        /// Converts the specified cast.
        /// </summary>
        /// <param name="cast">The cast.</param>
        /// <returns>A list of <see cref="CastMemberForJson"/>.</returns>
        public static List<CastMemberForJson> Convert(IEnumerable<CastMember> cast)
        {
            return cast
                .OrderByDescending(m => m.Birthdate)
                .Select(m => new CastMemberForJson
                {
                    Id = m.MemberId,
                    Name = m.Name,
                    Birthdate = m.Birthdate,
                })
                .ToList();
        }
    }
}