// <copyright file="AutofacConfig.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper
{
    using System.Web.Http;
    using System.Web.Mvc;
    using Autofac;
    using Autofac.Integration.Mvc;
    using Autofac.Integration.WebApi;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Services;
    using RtlTvMazeScraper.Infrastructure.Repositories.Local;
    using RtlTvMazeScraper.Infrastructure.Repositories.Remote;

    /// <summary>
    /// Configuration for the Autofac DI Container.
    /// </summary>
    public static class AutofacConfig
    {
        /// <summary>
        /// Registers this instance.
        /// </summary>
        public static void Register()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterApiControllers(typeof(MvcApplication).Assembly);

            builder.RegisterType<SettingRepository>().As<ISettingRepository>();
            builder.RegisterType<ShowRepository>().As<IShowRepository>();
            builder.RegisterType<ApiRepository>().As<IApiRepository>();
            builder.RegisterType<LogDebugRepository>().As<ILogRepository>();

            builder.RegisterType<TvMazeService>().As<ITvMazeService>();
            builder.RegisterType<ShowService>().As<IShowService>();

            var container = builder.Build();

            // MVC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // WebAPI
            var config = GlobalConfiguration.Configuration;
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}