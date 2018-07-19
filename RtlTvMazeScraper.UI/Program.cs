// <copyright file="Program.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.UI
{
    using System.IO;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

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
            var webhost = BuildWebHost(args);

            webhost.Run();
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
                .Build();
    }
}
