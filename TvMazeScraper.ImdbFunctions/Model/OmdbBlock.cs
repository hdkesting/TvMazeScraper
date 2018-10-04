﻿// <copyright file="OmdbBlock.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.ImdbFunctions.Model
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// A config entry to state whether OMDb is blocked.
    /// </summary>
    /// <seealso cref="Microsoft.WindowsAzure.Storage.Table.TableEntity" />
    public class OmdbBlock : TableEntity
    {
        /// <summary>
        /// Gets or sets the date until Omdb is considered blocked.
        /// </summary>
        /// <value>
        /// The blocked-until date.
        /// </value>
        public DateTimeOffset BlockedUntil { get; set; }
    }
}
