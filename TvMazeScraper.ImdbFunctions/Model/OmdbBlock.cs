// <copyright file="OmdbBlock.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.ImdbFunctions.Model
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// A config entry to state whether OMDb is blocked.
    /// </summary>
    /// <remarks>
    /// This functions as a time-limited "circuit breaker" in the processing pipeline.
    /// </remarks>
    /// <seealso cref="Microsoft.WindowsAzure.Storage.Table.TableEntity" />
    public class OmdbBlock : TableEntity
    {
        /// <summary>
        /// The partition key for configuration.
        /// </summary>
        public const string ConfigPartitionKey = "config";

        /// <summary>
        /// The row key for the single <see cref="OmdbBlock"/> entry.
        /// </summary>
        public const string OmdbBlockKey = "omdb-block";

        /// <summary>
        /// Initializes a new instance of the <see cref="OmdbBlock"/> class.
        /// </summary>
        public OmdbBlock()
        {
            this.PartitionKey = ConfigPartitionKey;
            this.RowKey = OmdbBlockKey;
        }

        /// <summary>
        /// Gets or sets the date until Omdb is considered blocked.
        /// </summary>
        /// <value>
        /// The blocked-until date.
        /// </value>
        public DateTimeOffset BlockedUntil { get; set; }
    }
}
