﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TvMazeScraper.Infrastructure.Sql.Model;

namespace TvMazeScraper.Infrastructure.Sql.Migrations
{
    [DbContext(typeof(ShowContext))]
    [Migration("20180724144659_ShowCastMember")]
    partial class ShowCastMember
    {
        /// <summary>
        /// Implemented to builds the <see cref="Microsoft.EntityFrameworkCore.Migrations.Migration.TargetModel" />.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="Microsoft.EntityFrameworkCore.ModelBuilder" /> to use to build the model.</param>
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TvMazeScraper.Core.Model.CastMember", b =>
                {
                    b.Property<int>("Id");

                    b.Property<DateTime?>("Birthdate");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("CastMembers");
                });

            modelBuilder.Entity("TvMazeScraper.Core.Model.Show", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Shows");
                });

            modelBuilder.Entity("TvMazeScraper.Core.Model.ShowCastMember", b =>
                {
                    b.Property<int>("ShowId");

                    b.Property<int>("CastMemberId");

                    b.HasKey("ShowId", "CastMemberId");

                    b.HasIndex("CastMemberId");

                    b.ToTable("ShowCastMembers");
                });

            modelBuilder.Entity("TvMazeScraper.Core.Model.ShowCastMember", b =>
                {
                    b.HasOne("TvMazeScraper.Core.Model.CastMember", "CastMember")
                        .WithMany("ShowCastMembers")
                        .HasForeignKey("CastMemberId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TvMazeScraper.Core.Model.Show", "Show")
                        .WithMany("ShowCastMembers")
                        .HasForeignKey("ShowId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
