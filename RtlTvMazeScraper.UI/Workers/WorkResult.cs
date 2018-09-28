// <copyright file="WorkResult.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.UI.Workers
{
    /// <summary>
    /// Result of a workunit.
    /// </summary>
    public enum WorkResult
    {
        /// <summary>Unkown result.</summary>
        Unkown,

        /// <summary>Work is done, one show is read.</summary>
        Done,

        /// <summary>No work, the queue is empty.</summary>
        Empty,

        /// <summary>Work postponed, the server was busy.</summary>
        Busy,

        /// <summary>Some error (see log).</summary>
        Error,
    }
}
