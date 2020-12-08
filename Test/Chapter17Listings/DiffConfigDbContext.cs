// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;

public enum DiffConfigs {Nothing, AddSchema, AddSequence, SetDefaultCol, SetComputedCol}

namespace Test.Chapter17Listings
{
    public class DiffConfigDbContext : DbContext
    {
        public DiffConfigs Config { get; private set; }

        public DiffConfigDbContext(
            DbContextOptions<DiffConfigDbContext> options, DiffConfigs config)      
            : base(options)
        {
            Config = config;
        }

        public DbSet<MyEntity> MyEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            switch (Config)
            {
                case DiffConfigs.Nothing:
                    break;
                case DiffConfigs.AddSchema:
                    modelBuilder.Entity<MyEntity>().ToTable("MyEntities", "MySchema");
                    break;
                case DiffConfigs.AddSequence:
                    modelBuilder.HasSequence<int>(
                        "OrderNumbers", "shared");
                    break;
                case DiffConfigs.SetDefaultCol:
                    modelBuilder.Entity<MyEntity>()
                        .Property(x => x.MyDateTime)
                        .HasDefaultValue(new DateTime(2000, 1, 1));
                    break;
                case DiffConfigs.SetComputedCol:
                    modelBuilder.Entity<MyEntity>()
                        .Property(p => p.MyInt)
                        .HasComputedColumnSql(
                            "DatePart(yyyy, [MyDateTime])");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}