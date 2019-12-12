using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using NewFileOrder.Models.DbModels;

namespace NewFileOrder.Models
{
    class MyDbContext : DbContext

    {
        public DbSet<FileModel> Files { get; set; }
        public DbSet<DirectoryModel> Directories { get; set; }
        public DbSet<TagModel> Tags { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=nfodata.sqlite");
    }
}
