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
    [Migration("20180629063744_InitialCreate")]
    partial class InitialCreate
    {
        /// <summary>
        /// Implemented to builds the <see cref="P:Microsoft.EntityFrameworkCore.Migrations.Migration.TargetModel" />.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="T:Microsoft.EntityFrameworkCore.ModelBuilder" /> to use to build the model.</param>
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TvMazeScraper.Core.Model.CastMember", b =>
                {
                    b.Property<int>("ShowId");

                    b.Property<int>("MemberId");

                    b.Property<DateTime?>("Birthdate");

                    b.Property<string>("Name");

                    b.HasKey("ShowId", "MemberId");

                    b.HasAlternateKey("MemberId", "ShowId");

                    b.ToTable("CastMembers");
                });

            modelBuilder.Entity("TvMazeScraper.Core.Model.Show", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Shows");
                });

            modelBuilder.Entity("TvMazeScraper.Core.Model.CastMember", b =>
                {
                    b.HasOne("TvMazeScraper.Core.Model.Show")
                        .WithMany("CastMembers")
                        .HasForeignKey("ShowId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
