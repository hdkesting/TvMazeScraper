// <copyright file="LogDebugRepository.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Infrastructure.Repositories.Local
{
    using System;
    using System.Runtime.CompilerServices;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Support;

    /// <summary>
    /// Repository to send log messages to.
    /// </summary>
    /// <remarks>
    /// Could also have logged to some more persistent target.
    /// </remarks>
    /// <seealso cref="RtlTvMazeScraper.Core.Interfaces.ILogRepository" />
    public class LogDebugRepository : ILogRepository
    {
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception (if any).</param>
        /// <param name="methodName">Name of the method (automatically filled).</param>
        public void Log(LogLevel logLevel, string message, Exception exception = null, [CallerMemberName] string methodName = null)
        {
            if (exception == null)
            {
                System.Diagnostics.Debug.WriteLine($"{logLevel} [{methodName}] - {message}.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"{logLevel} [{methodName}] - {message}:");

                while (exception != null)
                {
                    System.Diagnostics.Debug.WriteLine(exception);
                    exception = exception.InnerException;
                }

                System.Diagnostics.Debug.WriteLine("====");
            }
        }
    }
}
