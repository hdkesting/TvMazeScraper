// <copyright file="LogLevel.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Support
{
    /// <summary>
    /// Logging levels.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// The debug level: many low level messages.
        /// </summary>
        Debug,

        /// <summary>
        /// The information level: just information.
        /// </summary>
        Information,

        /// <summary>
        /// The warning level: maybe check this out?
        /// </summary>
        Warning,

        /// <summary>
        /// The error level: action is needed. Usually an exception should be added.
        /// </summary>
        Error,
    }
}
