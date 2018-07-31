// <copyright file="Program.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.UI
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using NLog.Web;

    /// <summary>
    /// The main application entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The commandline arguments.</param>
        public static void Main(string[] args)
        {
            // https://github.com/NLog/NLog.Web/wiki/Getting-started-with-ASP.NET-Core-2
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                var webhost = BuildWebHost(args);

                webhost.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Webapp halted because of exception.");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        /// <summary>
        /// Builds the web host.
        /// </summary>
        /// <param name="args">The commandline arguments.</param>
        /// <returns>An <see cref="IWebHost"/>.</returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseNLog()
                .Build();
    }
}
