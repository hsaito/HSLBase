using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using HSLProcessor;

namespace HSLProcessor.Migrations
{
    [DbContext(typeof(HSLContext))]
    [Migration("20170523051132_Songs")]
    partial class Songs
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("HSLProcessor.Artist", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("HSLProcessor.Series", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Series");
                });

            modelBuilder.Entity("HSLProcessor.Song", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ArtistForeignKey");

                    b.Property<Guid>("SourceForeignKey");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ArtistForeignKey");

                    b.HasIndex("SourceForeignKey");

                    b.ToTable("Songs");
                });

            modelBuilder.Entity("HSLProcessor.Source", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid?>("SeriesId");

                    b.HasKey("Id");

                    b.HasIndex("SeriesId");

                    b.ToTable("Sources");
                });

            modelBuilder.Entity("HSLProcessor.Song", b =>
                {
                    b.HasOne("HSLProcessor.Artist", "Artist")
                        .WithMany()
                        .HasForeignKey("ArtistForeignKey")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HSLProcessor.Source", "Source")
                        .WithMany()
                        .HasForeignKey("SourceForeignKey")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HSLProcessor.Source", b =>
                {
                    b.HasOne("HSLProcessor.Series")
                        .WithMany("Source")
                        .HasForeignKey("SeriesId");
                });
        }
    }
}
