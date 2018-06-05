// <copyright file="Global.asax.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper
{
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

#pragma warning disable SA1649 // File name should match first type name
    /// <summary>
    /// The MVC application.
    /// </summary>
    /// <seealso cref="System.Web.HttpApplication" />
    public class MvcApplication : System.Web.HttpApplication
#pragma warning restore SA1649 // File name should match first type name
    {
        /// <summary>
        /// Called on application start, to initialize everything.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutofacConfig.Register();
        }
    }
}
