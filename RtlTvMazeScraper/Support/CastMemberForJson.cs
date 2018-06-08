// <copyright file="CastMemberForJson.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Support
{
    using System;
    using System.Globalization;
    using Newtonsoft.Json;

    /// <summary>
    /// Cast member details, for JSON serialisation.
    /// </summary>
    public class CastMemberForJson
    {
        /// <summary>
        /// Gets or sets the member identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the birthdate.
        /// </summary>
        /// <value>
        /// The birthdate.
        /// </value>
        [JsonIgnore]
        public DateTime? Birthdate { get; set; }

        /// <summary>
        /// Gets or sets the birthday as string.
        /// </summary>
        /// <value>
        /// The birthday.
        /// </value>
        public string Birthday
        {
            get
            {
                return this.Birthdate?.ToString("yyyy-MM-dd");
            }

            set
            {
                if (!string.IsNullOrWhiteSpace(value) && DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                {
                    this.Birthdate = dt;
                }
                else
                {
                    this.Birthdate = null;
                }
            }
        }
    }
}