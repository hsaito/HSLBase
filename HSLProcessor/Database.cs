using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace HSLProcessor
{
    public class Song
    {
        [Key]
        public Guid Id;

        public string Title;
        public string Artist;
        public string Reference;
    }

    public class HSLContext : DbContext
    {
        public DbSet<Song> Songs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=HSL.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Song>().HasKey("Id");
        }
    }
}