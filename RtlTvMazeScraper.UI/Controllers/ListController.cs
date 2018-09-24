// <copyright file="ListController.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.UI.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RtlTvMazeScraper.Core.DTO;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.UI.ViewModels;

    /// <summary>
    /// Give list of shows and their cast.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    public class ListController : Controller
    {
        private readonly IShowService showService;
        private readonly IMapper mapper;
        private readonly ILogger<ListController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListController" /> class.
        /// </summary>
        /// <param name="showService">The show service.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="logger">The logger.</param>
        public ListController(
            IShowService showService,
            IMapper mapper,
            ILogger<ListController> logger)
        {
            this.showService = showService;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the shows as JSON-serializable objects.
        /// </summary>
        /// <param name="pageno">The page number (starts at 0).</param>
        /// <param name="pagesize">The number of shows per page.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A JSON-serializable list.
        /// </returns>
        /// <remarks>
        /// Note that some configuration is done in WebApiConfig.cs.
        /// </remarks>
        [HttpGet]
        [Route("api/list", Name = "apilist")]
        public async Task<List<ShowForJson>> List(int pageno = 0, int pagesize = 20, CancellationToken cancellationToken = default)
        {
            if (pageno < 0)
            {
                pageno = 0;
            }

            if (pagesize < 2)
            {
                pagesize = 2;
            }

            var dbshows = await this.showService.GetShowsWithCast(pageno, pagesize, cancellationToken).ConfigureAwait(false);
            this.logger.Log(LogLevel.Information, "Found {PageCount} shows for {PageNumber} ({PageSize})", dbshows.Count, pageno, pagesize);

            var result = this.mapper.Map<List<ShowDto>, List<ShowForJson>>(dbshows);

            // per requirement, sort descending by birthday
            result.ForEach(s => s.Cast = s.Cast.OrderByDescending(cm => cm.Birthdate).ToList());

            return result;
        }
    }
}