// <copyright file="Startup.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.UI
{
    using System;
    using System.Net.Http;
    using AutoMapper;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Polly;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Model;
    using RtlTvMazeScraper.Core.Services;
    using RtlTvMazeScraper.Infrastructure.Repositories.Local;
    using RtlTvMazeScraper.Infrastructure.Repositories.Remote;
    using RtlTvMazeScraper.UI.ViewModels;

    /// <summary>
    /// The initialization class of the web app.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

            services.AddDbContext<ShowContext>(opt => opt.UseSqlServer(
                this.Configuration.GetConnectionString("ShowConnection"),
                x => x.MigrationsAssembly("RtlTvMazeScraper.Infrastructure")));

            var retryPolicy = Policy.HandleResult<HttpResponseMessage>(resp => resp.StatusCode == Core.Support.Constants.ServerTooBusy)
                                    .WaitAndRetryAsync(new[]
                                    {
                                        TimeSpan.FromSeconds(5),
                                        TimeSpan.FromSeconds(10),
                                        TimeSpan.FromSeconds(20),
                                    });
            var host = this.Configuration.GetSection("Config")["tvmaze"];

            // on http status 429, wait and retry (using Polly)
            services.AddHttpClient(Core.Support.Constants.TvMazeClientWithRetry, client =>
            {
                client.BaseAddress = new Uri(host);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddPolicyHandler(retryPolicy);

            this.ConfigureDI(services);
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
#pragma warning restore CA1822 // Mark members as static
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // static files
            ////app.UseDefaultFiles();
            ////app.UseStaticFiles(new StaticFileOptions { OnPrepareResponse = this.PrepareCompressedResponse });

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static MapperConfiguration ConfigureMapping()
        {
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Show, ShowForJson>()
                    .ForMember(dest => dest.Cast, opt => opt.MapFrom(src => src.CastMembers));
                cfg.CreateMap<CastMember, CastMemberForJson>();
            });

            return config;
        }

        /// <summary>
        /// Configures the dependency injector.
        /// </summary>
        /// <param name="services">The services.</param>
        private void ConfigureDI(IServiceCollection services)
        {
            // repositories
            services.AddTransient<IApiRepository, ApiRepository>();
            services.AddTransient<IShowRepository, ShowRepository>();
            services.AddSingleton<ISettingRepository, SettingRepository>(sp => new SettingRepository(this.Configuration));

            // services
            services.AddTransient<IShowService, ShowService>();
            services.AddTransient<ITvMazeService, TvMazeService>();

            // db context
            services.AddTransient<IShowContext, ShowContext>();

            // other
            var mappingConfig = ConfigureMapping();
            services.AddSingleton<IMapper, IMapper>(sp => mappingConfig.CreateMapper());
        }
    }
}
