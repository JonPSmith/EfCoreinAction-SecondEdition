// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter09Listings.SeedExample
{
    public class SeedExampleDbContext : DbContext
    {
        public SeedExampleDbContext(
            DbContextOptions<SeedExampleDbContext> options)
            : base(options)
        { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().HasData(
                new { ProjectId = 1, ProjectName = "Project1"}, 
                new { ProjectId = 2, ProjectName = "Project2"});
            modelBuilder.Entity<User>().HasData(
                new { UserId = 1, Name = "Jill", ProjectId = 1 },
                new { UserId = 2, Name = "Jack", ProjectId = 2 });
            modelBuilder.Entity<User>()
                .OwnsOne(x => x.Address).HasData(
                    new {UserId = 1, Street = "Jill street", City = "city1"},
                    new {UserId = 2, Street = "Jack street", City = "city2"});
        }
    }
    /**********************************************************
     To create a migration I had to
    1. Set the startup project to Test
    2. In PMC type:  Add-Migration Initial -context SeedExampleDbContext -OutputDir SeedExampleMigrations
     **********************************************************/
}