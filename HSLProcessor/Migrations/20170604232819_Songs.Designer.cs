using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using HSLProcessor;

namespace HSLProcessor.Migrations
{
    [DbContext(typeof(HSLContext))]
    [Migration("20170604232819_Songs")]
    partial class Songs
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

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
        }
    }
}
