// <copyright file="FilterConfig.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper
{
    using System.Web.Mvc;

    /// <summary>
    /// Configuration for default controller filtering.
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Registers the global filters.
        /// </summary>
        /// <param name="filters">The filters.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
