using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSLProcessor
{
    [Table("Song")]
    public class Song
    {
        [Required, Key]
        public Guid TitleId { get; set; }

        [Required]
        public string Title { get; set; }

        public Guid ArtistId { get; set; }
        [ForeignKey("ArtistId")]
        public virtual Artist Artist { get; set; }

        public Guid SourceId { get; set; }
        [ForeignKey("SourceId")]
        public virtual Source Source { get; set; }
    }

    [Table("Artist")]
    public class Artist
    {
        [Required, Key]
        public Guid ArtistId { get; set; }
        [Required]
        public string Name { get; set; }
    }

    [Table("Source")]
    public class Source
    {
        [Required, Key]
        public Guid SourceId { get; set; }
        [Required]
        public string Name { get; set; }
    }

    [Table("Series")]
    public class Series
    {
        [Required, Key]
        public Guid SeriesId { get; set; }
        [Required]
        public string Name { get; set; }
        public List<Source> Source { get; set; }
    }

    public class HSLContext : DbContext
    {
        public virtual DbSet<Song> Songs { get; set; }
        public virtual DbSet<Artist> Artists { get; set; }
        public virtual DbSet<Source> Sources { get; set; }

        /// <summary>
        /// Explicitly load tables
        /// </summary>
        public void LoadRelations()
        {
            foreach(var song in Songs)
            {
                song.Artist = Artists.Find(song.ArtistId);
                song.Source = Sources.Find(song.SourceId);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=HSL.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Song>().HasKey("TitleId");
            modelBuilder.Entity<Artist>().HasKey("ArtistId");
            modelBuilder.Entity<Source>().HasKey("SourceId");
            modelBuilder.Entity<Song>().ToTable("Songs");
            modelBuilder.Entity<Artist>().ToTable("Artists");
            modelBuilder.Entity<Source>().ToTable("Sources");
            base.OnModelCreating(modelBuilder);
        }
    }
}