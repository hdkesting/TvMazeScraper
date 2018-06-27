// <copyright file="DebugLogger.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Test.Mock
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Logger to the Debug window, for use in unittests.
    /// </summary>
    /// <typeparam name="T">Class to log for.</typeparam>
    /// <seealso cref="Microsoft.Extensions.Logging.ILogger{T}" />
    public class DebugLogger<T> : ILogger<T>
    {
        /// <summary>
        /// Begins the scope.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="state">The state.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="NotImplementedException">Not implemented.</exception>
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the specified log level is enabled.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>
        ///   <c>true</c> if the specified log level is enabled; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        /// <summary>
        /// Logs at the specified log level.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="logLevel">The log level.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="state">The state.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="formatter">The formatter.</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var msg = formatter(state, exception);
            System.Diagnostics.Debug.WriteLine($"{typeof(T).Name} [{logLevel}] - {msg}.");
        }
    }
}
