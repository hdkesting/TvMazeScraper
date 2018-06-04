using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using RtlTvMazeScraper.Interfaces;
using RtlTvMazeScraper.Repositories;
using RtlTvMazeScraper.Services;

namespace RtlTvMazeScraper
{
    public static class AutofacConfig
    {
        public static void Register()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterApiControllers(typeof(MvcApplication).Assembly);

            builder.RegisterType<SettingRepository>().As<ISettingRepository>();
            builder.RegisterType<ShowRepository>().As<IShowRepository>();

            builder.RegisterType<TvMazeService>().As<ITvMazeService>();

            var container = builder.Build();

            // MVC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // WebAPI
            var config = GlobalConfiguration.Configuration;
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}