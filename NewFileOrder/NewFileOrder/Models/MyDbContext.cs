﻿using System;
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
        public DbSet<File> Files { get; set; }
        public DbSet<Directory> Directories{get;set;}
               protected override void OnConfiguring(DbContextOptionsBuilder options)
                    => options.UseSqlite("Data Source=TestDatabase.db");
    }
}
