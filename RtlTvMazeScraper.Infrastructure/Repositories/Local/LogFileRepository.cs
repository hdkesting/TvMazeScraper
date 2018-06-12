// <copyright file="LogFileRepository.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Infrastructure.Repositories.Local
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Support;

    /// <summary>
    /// Logger that logs to a file.
    /// </summary>
    /// <seealso cref="RtlTvMazeScraper.Core.Interfaces.ILogRepository" />
    public class LogFileRepository : ILogRepository
    {
        private const string LogPath = @"C:\Temp\Scraper";
        private readonly string filename;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogFileRepository" /> class.
        /// </summary>
        public LogFileRepository()
        {
            var now = DateTime.Now;
            Directory.CreateDirectory(LogPath);
            this.filename = Path.Combine(LogPath, $"Logfile_{now: yyyy-MM-dd_HH-mm-ss}.txt");
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception (if any).</param>
        /// <param name="methodName">Name of the method (automatically filled).</param>
        public void Log(LogLevel logLevel, string message, Exception exception = null, [CallerMemberName] string methodName = null)
        {
            StringBuilder output = new StringBuilder();

            output.Append($"{DateTime.Now.ToString("HH:mm:ss.f")} {logLevel} [{methodName}] - {message}");
            if (exception != null)
            {
                while (exception != null)
                {
                    output.AppendLine(exception.ToString());
                    exception = exception.InnerException;
                }

                output.AppendLine("====");
            }

            using (var sw = TextWriter.Synchronized(File.AppendText(this.filename)))
            {
                sw.WriteLine(output.ToString());
            }
        }
    }
}
