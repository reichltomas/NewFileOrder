using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using NewFileOrder.Models.DbModels;

namespace NewFileOrder.Models
{
    public class MyDbContext : DbContext

    {
        public DbSet<FileModel> Files { get; set; }
        public DbSet<DirectoryModel> Directories { get; set; }
        public DbSet<TagModel> Tags { get; set; }

        public DbSet<FileTag> FileTags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=nfodata.sqlite");

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<FileTag>().HasKey(ft => new { ft.FileId, ft.TagId });

            builder.Entity<FileTag>().HasOne<FileModel>(ft => ft.File)
                                     .WithMany(f => f.FileTags)
                                     .HasForeignKey(ft => ft.FileId);

            builder.Entity<FileTag>().HasOne<TagModel>(ft => ft.Tag)
                                     .WithMany(t => t.FileTags)
                                     .HasForeignKey(ft => ft.TagId);
        }
    }
}
