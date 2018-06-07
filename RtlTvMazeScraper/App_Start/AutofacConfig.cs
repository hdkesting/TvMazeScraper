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
    using RtlTvMazeScraper.Infrastructure.Data;
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

            builder.RegisterType<ShowContext>().As<IShowContext>();

            builder.RegisterType<SettingRepository>().As<ISettingRepository>();
            builder.RegisterType<ShowRepository>().As<IShowRepository>();
            builder.RegisterType<ApiRepository>().As<IApiRepository>();
            builder.RegisterType<LogFileRepository>().As<ILogRepository>().SingleInstance();

            builder.RegisterType<TvMazeService>().As<ITvMazeService>();
            builder.RegisterType<ShowService>().As<IShowService>();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterApiControllers(typeof(MvcApplication).Assembly);

            var container = builder.Build();

            // MVC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // WebAPI
            var config = GlobalConfiguration.Configuration;
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}