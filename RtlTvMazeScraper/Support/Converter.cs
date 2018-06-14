// <copyright file="Converter.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Support
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Linq;
    using RtlTvMazeScraper.Core.Model;

    /// <summary>
    /// Converts to JSON-serializable format.
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Converts the specified shows to JSON-serializable classes.
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
        /// Converts the specified cast to JSON-serializable classes.
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

        /// <summary>
        /// Converts the shows to a <see cref="JArray"/> for serializing to JSON.
        /// </summary>
        /// <param name="shows">The shows.</param>
        /// <returns>A JArray containing the show data.</returns>
        public static JArray ShowsToJArray(List<Show> shows)
        {
            JArray result = new JArray();

            foreach (var show in shows)
            {
                var cast = new JArray();

                // order cast by birthdate, descending. As per requirement.
                foreach (var member in show.CastMembers.OrderByDescending(m => m.Birthdate))
                {
                    var cm = new JObject(
                        new JProperty("id", member.MemberId),
                        new JProperty("name", member.Name),
                        new JProperty("birthday", member.Birthdate.HasValue ? member.Birthdate.Value.ToString("yyyy-MM-dd") : null));
                    cast.Add(cm);
                }

                var showObj = new JObject(
                    new JProperty("id", show.Id),
                    new JProperty("name", show.Name),
                    new JProperty("cast", cast));

                result.Add(showObj);
            }

            return result;
        }
    }
}