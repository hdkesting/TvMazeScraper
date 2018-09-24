// <copyright file="CastMemberDto.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.DTO
{
    using System;

    /// <summary>
    /// A cast member.
    /// </summary>
    public sealed class CastMemberDto
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
        public DateTime? Birthdate { get; set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(CastMemberDto)} {this.Name} ({this.Birthdate}) - ID {this.Id}";
        }
    }
}