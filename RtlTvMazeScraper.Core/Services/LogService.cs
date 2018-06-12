// <copyright file="LogService.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Services
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Support;

    /// <summary>
    /// Service for logging.
    /// </summary>
    public class LogService
    {
        private readonly ILogRepository logRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogService"/> class.
        /// </summary>
        /// <param name="logRepository">The log repository.</param>
        public LogService(ILogRepository logRepository)
        {
            this.logRepository = logRepository;
        }

        /// <summary>
        /// Logs the specified bebug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="methodName">Name of the method (automatically filled).</param>
        [Conditional("DEBUG")]
        public void LogDebug(string message, [CallerMemberName] string methodName = null)
        {
            this.logRepository.Log(LogLevel.Debug, message, null, methodName);
        }

        /// <summary>
        /// Logs the specified information message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="methodName">Name of the method (automatically filled).</param>
        public void LogInformation(string message, [CallerMemberName] string methodName = null)
        {
            this.logRepository.Log(LogLevel.Information, message, null, methodName);
        }

        /// <summary>
        /// Logs the specified warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception (if any).</param>
        /// <param name="methodName">Name of the method (automatically filled).</param>
        public void LogWarning(string message, Exception exception = null, [CallerMemberName] string methodName = null)
        {
            this.logRepository.Log(LogLevel.Warning, message, exception, methodName);
        }

        /// <summary>
        /// Logs the specified error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception (if any).</param>
        /// <param name="methodName">Name of the method (automatically filled).</param>
        public void LogError(string message, Exception exception = null, [CallerMemberName] string methodName = null)
        {
            this.logRepository.Log(LogLevel.Error, message, exception, methodName);
        }

    }
}
