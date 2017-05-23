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
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }
        public Guid ArtistForeignKey;
        [ForeignKey("ArtistForeignKey")]
        public Artist Artist { get; set; }
        public Guid SourceForeignKey;
        [ForeignKey("SourceForeignKey")]
        public Source Source { get; set; }
    }

    [Table("Artist")]
    public class Artist
    {
        [Required, Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
    }

    [Table("Source")]
    public class Source
    {
        [Required, Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
    }

    [Table("Series")]
    public class Series
    {
        [Required, Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<Source> Source {get; set; }
}

public class HSLContext : DbContext
{
    public virtual DbSet<Song> Songs { get; set; }
    public virtual DbSet<Artist> Artists { get; set; }
    public virtual DbSet<Source> Sources { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=HSL.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Song>().HasKey("Id");
        modelBuilder.Entity<Artist>().HasKey("Id");
        modelBuilder.Entity<Source>().HasKey("Id");
        modelBuilder.Entity<Song>().ToTable("Songs");
        modelBuilder.Entity<Artist>().ToTable("Artists");
        modelBuilder.Entity<Source>().ToTable("Sources");
        base.OnModelCreating(modelBuilder);
    }
}
}