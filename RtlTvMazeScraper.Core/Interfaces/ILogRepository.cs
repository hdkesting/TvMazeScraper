// <copyright file="ILogRepository.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Interfaces
{
    using System;
    using System.Runtime.CompilerServices;
    using RtlTvMazeScraper.Core.Support;

    /// <summary>
    /// Repostory to send log messages to.
    /// </summary>
    public interface ILogRepository
    {
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception (if any).</param>
        /// <param name="methodName">Name of the method (automatically filled).</param>
        void Log(LogLevel logLevel, string message, Exception exception = null, [CallerMemberName] string methodName = null);
    }
}
