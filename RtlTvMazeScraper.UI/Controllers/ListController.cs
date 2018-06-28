// <copyright file="ListController.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.UI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Model;
    using RtlTvMazeScraper.UI.ViewModels;

    /// <summary>
    /// Give list of shows and their cast.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiController]
    public class ListController : ControllerBase
    {
        private readonly IShowService showService;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListController" /> class.
        /// </summary>
        /// <param name="showService">The show service.</param>
        /// <param name="mapper">The mapper.</param>
        public ListController(
            IShowService showService,
            IMapper mapper)
        {
            this.showService = showService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets the shows as JSON-serializable objects.
        /// </summary>
        /// <remarks>
        /// Note that some configuration is done in WebApiConfig.cs.
        /// </remarks>
        /// <param name="page">The page.</param>
        /// <param name="pagesize">The pagesize.</param>
        /// <returns>A JSON-serializable list.</returns>
        [HttpGet]
        [Route("api/list")]
        public async Task<List<ShowForJson>> List(int page = 0, int pagesize = 20)
        {
            if (page < 0)
            {
                page = 0;
            }

            if (pagesize < 2)
            {
                pagesize = 2;
            }

            var dbshows = await this.showService.GetShowsWithCast(page, pagesize).ConfigureAwait(false);

            var result = this.mapper.Map<List<Show>, List<ShowForJson>>(dbshows);

            // per requirement, sort descending by birthday
            result.ForEach(s => s.Cast = s.Cast.OrderByDescending(cm => cm.Birthdate).ToList());

            return result;
        }
    }
}