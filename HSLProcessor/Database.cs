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
        [Required]
        public string Artist { get; set; }
        public string Source { get; set; }
    }

    public class HSLContext : DbContext
    {
        public virtual DbSet<Song> Songs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=HSL.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Song>().HasKey("Id");
            modelBuilder.Entity<Song>().ToTable("Songs");
            base.OnModelCreating(modelBuilder);
        }
    }
}