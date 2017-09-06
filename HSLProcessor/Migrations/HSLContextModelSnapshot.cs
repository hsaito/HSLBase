﻿// <auto-generated />
using HSLProcessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace HSLProcessor.Migrations
{
    [DbContext(typeof(HSLContext))]
    partial class HSLContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452");

            modelBuilder.Entity("HSLProcessor.Artist", b =>
                {
                    b.Property<Guid>("ArtistId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ArtistId");

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("HSLProcessor.Series", b =>
                {
                    b.Property<Guid>("SeriesId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("SeriesId");

                    b.ToTable("Series");
                });

            modelBuilder.Entity("HSLProcessor.Song", b =>
                {
                    b.Property<Guid>("TitleId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ArtistId");

                    b.Property<Guid>("SourceId");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("TitleId");

                    b.HasIndex("ArtistId");

                    b.HasIndex("SourceId");

                    b.ToTable("Songs");
                });

            modelBuilder.Entity("HSLProcessor.Source", b =>
                {
                    b.Property<Guid>("SourceId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid?>("SeriesId");

                    b.HasKey("SourceId");

                    b.HasIndex("SeriesId");

                    b.ToTable("Sources");
                });

            modelBuilder.Entity("HSLProcessor.Song", b =>
                {
                    b.HasOne("HSLProcessor.Artist", "Artist")
                        .WithMany()
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HSLProcessor.Source", "Source")
                        .WithMany()
                        .HasForeignKey("SourceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HSLProcessor.Source", b =>
                {
                    b.HasOne("HSLProcessor.Series", "Series")
                        .WithMany()
                        .HasForeignKey("SeriesId");
                });
#pragma warning restore 612, 618
        }
    }
}
