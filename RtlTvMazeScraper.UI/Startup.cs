// <copyright file="Startup.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.UI
{
    using System;
    using System.Net.Http;
    using AutoMapper;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Polly;
    using TvMazeScraper.Core.Interfaces;
    using TvMazeScraper.Core.Services;
    using TvMazeScraper.Core.Support;
    using TvMazeScraper.Core.Support.Events;
    using TvMazeScraper.Core.Workers;
    using TvMazeScraper.Infrastructure.Repositories.Local;
    using TvMazeScraper.Infrastructure.Repositories.Remote;
    using TvMazeScraper.Infrastructure.Sql.Model;
    using TvMazeScraper.UI.Hubs;
    using TvMazeScraper.UI.ViewModels;
    using TvMazeScraper.UI.Workers;

    /// <summary>
    /// The initialization class of the web app.
    /// </summary>
    public class Startup
    {
        // https://codingblast.com/asp-net-core-configureservices-vs-configure/

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="env">The hosting environment.</param>
        public Startup(Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.1&tabs=windows
                builder.AddUserSecrets<Startup>();
            }

            this.Configuration = builder.Build();
        }

        /// <summary>
        /// Where the shows should be persisted.
        /// </summary>
        private enum Storage
        {
            Unknown,
            Sql,
            MongoDB,
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; }

#pragma warning disable CA1506 // Avoid excessive class coupling
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
#pragma warning restore CA1506 // Avoid excessive class coupling
        {
            services.AddMvc()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

            services.AddLogging(builder =>
            {
                builder.AddConfiguration(this.Configuration.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();
            });

            switch (this.GetStorageType())
            {
                case Storage.Sql:
                    services.AddDbContext<ShowContext>(opt => opt.UseSqlServer(
                        this.Configuration.GetConnectionString("ShowConnection"),
                        x => x.MigrationsAssembly("TvMazeScraper.Infrastructure.Sql")));
                    break;

                case Storage.MongoDB:
                    Infrastructure.Mongo.Startup.Configure(this.Configuration.GetConnectionString("MongoConnection"));
                    break;

                default:
                    throw new InvalidOperationException("Unknown storage type");
            }

            // "at least 20 calls every 10 seconds"
            var retryPolicy = Policy.HandleResult<HttpResponseMessage>(resp => resp.StatusCode == Constants.ServerTooBusy)
                                    .WaitAndRetryAsync(new[]
                                    {
                                        TimeSpan.FromSeconds(5),
                                        TimeSpan.FromSeconds(5),
                                        TimeSpan.FromSeconds(10),
                                    });
            var host1 = this.Configuration.GetSection("Config")["tvmaze"];

            // TVMaze: on http status 429, wait and retry (using Polly)
            services.AddHttpClient(Constants.TvMazeClientWithRetry, client =>
            {
                client.BaseAddress = new Uri(host1);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddPolicyHandler(retryPolicy);

            // OMDb: no retry policy useful (max 1000 per day) - obsolete (use own microservice)
            var host2 = this.Configuration.GetSection("Config")["omdbapi"];
            services.AddHttpClient(Constants.OmdbClient, client =>
            {
                client.BaseAddress = new Uri(host2);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // OMDb microservice
            var host3 = this.Configuration.GetSection("Config")["omdbsvc"];
            services.AddHttpClient(Constants.OmdbMicroService, client =>
            {
                client.BaseAddress = new Uri(host3);
            });

            services.AddSignalR();

            this.ConfigureDependencyInjection(services);

            ConfigureSubscriptions();
        }

#pragma warning disable CA1822 // Mark members as static
        /// <summary>
        /// Configures this application.
        /// </summary>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </remarks>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
#pragma warning restore CA1822 // Mark members as static
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // static files
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseSignalR(routes => { routes.MapHub<ScraperHub>("/scraperHub"); });

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            // apikey is stored as "secret"
            OmdbService.ApiKey = this.Configuration["omdbApiKey"] ?? throw new InvalidOperationException("The secret omdbApiKey is missing");
        }

        /// <summary>
        /// Configures the mapping between various types.
        /// </summary>
        /// <returns>A configuration.</returns>
        private static MapperConfiguration ConfigureMapping()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Core.DTO.ShowDto, ShowForJson>()
                    .ForMember(dest => dest.Cast, opt => opt.MapFrom(src => src.CastMembers));
                cfg.CreateMap<Core.DTO.CastMemberDto, CastMemberForJson>();
            });

            return config;
        }

        /// <summary>
        /// Configures the <see cref="MessageHub"/> subscriptions.
        /// </summary>
        private static void ConfigureSubscriptions()
        {
            MessageHub.Subscribe<IOmdbService, ShowStoredEvent>(nameof(IOmdbService.EnrichShowWithRating));
        }

        /// <summary>
        /// Configures the dependency injector.
        /// </summary>
        /// <param name="services">The services collection to add to.</param>
        private void ConfigureDependencyInjection(IServiceCollection services)
        {
            // repositories
            services.AddTransient<IApiRepository, ApiRepository>();
            services.AddSingleton<ISettingRepository, SettingRepository>(sp => new SettingRepository(this.Configuration));

            // services
            services.AddTransient<IMessageHub, MessageHub>();
            services.AddScoped<IShowService, ShowService>();
            services.AddScoped<ITvMazeService, TvMazeService>();

            services.AddTransient<IOmdbService, OmdbService>();

            var persistence = this.GetStorageType();
            switch (persistence)
            {
                case Storage.Sql:
                    Infrastructure.Sql.Startup.ConfigureDI(services);
                    break;

                case Storage.MongoDB:
                    Infrastructure.Mongo.Startup.ConfigureDI(services);
                    break;

                default:
                    throw new InvalidOperationException($"Persistence type not supported: {persistence}.");
            }

            // other
            var mappingConfig = ConfigureMapping();
            services.AddSingleton<IMapper, IMapper>(sp => mappingConfig.CreateMapper());

            // background services: signalR based scraper
            services.AddScoped<IScraperWorker, ScraperWorker>();
            services.AddHostedService<ShowLoaderHostedService>();

            // background services: rating queue processing
            services.AddScoped<IIncomingRatingProcessor, IncomingRatingProcessor>();
            services.AddTransient<IIncomingRatingRepository, IncomingRatingQueueRepository>(sp => new IncomingRatingQueueRepository(this.Configuration));
            services.AddHostedService<RatingQueueHostedService>();
        }

        private Storage GetStorageType()
        {
            var storage = this.Configuration.GetSection("Config")["persisting"];
            switch (storage)
            {
                case "sql":
                    return Storage.Sql;

                case "mongo":
                    return Storage.MongoDB;
            }

            throw new InvalidOperationException($"Wrong 'persisting' configuration. Expected 'sql' or 'mongo', but got '{storage}'.");
        }
    }
}
