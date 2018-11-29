// <copyright file="Startup.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Infrastructure.Sql
{
    using System.Linq;
    using AutoMapper;
    using Microsoft.Extensions.DependencyInjection;
    using TvMazeScraper.Core.Interfaces;
    using TvMazeScraper.Infrastructure.Sql.Interfaces;
    using TvMazeScraper.Infrastructure.Sql.Model;
    using TvMazeScraper.Infrastructure.Sql.Repositories;

    /// <summary>
    /// Statup as relates to this project.
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Configures the dependency injection, specific for this project.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void ConfigureDI(IServiceCollection services)
        {
            services.AddTransient<IShowRepository, ShowRepository>();

            // db context
            services.AddTransient<IShowContext, ShowContext>();
        }

        /// <summary>
        /// Configures AutoMapper for local types.
        /// </summary>
        /// <param name="cfg">The CFG.</param>
        public static void ConfigureMapping(IMapperConfigurationExpression cfg)
        {
            ////cfg.CreateMap<Core.DTO.ShowDto, ShowForJson>()
            ////    .ForMember(dest => dest.Cast, opt => opt.MapFrom(src => src.CastMembers));
            ////cfg.CreateMap<Core.DTO.CastMemberDto, CastMemberForJson>();

            cfg.CreateMap<Show, Core.DTO.ShowDto>();
            cfg.CreateMap<Core.DTO.ShowDto, Show>();

            cfg.CreateMap<CastMember, Core.DTO.CastMemberDto>();
            cfg.CreateMap<Core.DTO.CastMemberDto, CastMember>();

            // TODO cast members
        }
    }
}
