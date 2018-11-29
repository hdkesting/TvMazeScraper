// <copyright file="Startup.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Infrastructure.Mongo
{
    using AutoMapper;
    using Microsoft.Extensions.DependencyInjection;
    using TvMazeScraper.Core.Interfaces;
    using TvMazeScraper.Infrastructure.Mongo.Model;
    using TvMazeScraper.Infrastructure.Mongo.Repositories;

    /// <summary>
    /// Statup as relates to this project.
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        internal static string ConnectionString { get; private set; }

        /// <summary>
        /// Configures this project.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public static void Configure(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Configures the dependecy injection, specific for this project.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void ConfigureDI(IServiceCollection services)
        {
            services.AddTransient<IShowRepository, MongoShowRepository>(); // or singleton?
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

            cfg.CreateMap<ShowWithCast, Core.DTO.ShowDto>()
                .ForMember(sh => sh.CastMembers, opt => opt.MapFrom(swc => swc.Cast));
            cfg.CreateMap<Core.DTO.ShowDto, ShowWithCast>()
                .ForMember(swc => swc.Cast, opt => opt.MapFrom(sh => sh.CastMembers));
        }
    }
}
