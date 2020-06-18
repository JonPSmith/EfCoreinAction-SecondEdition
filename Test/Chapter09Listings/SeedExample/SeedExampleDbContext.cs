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

        protected override void OnModelCreating(ModelBuilder modelBuilder) //#A
        {
            modelBuilder.Entity<Project>().HasData(                        //#B
                new { ProjectId = 1, ProjectName = "Project1"},            //#B
                new { ProjectId = 2, ProjectName = "Project2"});           //#B
            modelBuilder.Entity<User>().HasData(                           //#C
                new { UserId = 1, Name = "Jill", ProjectId = 1 },          //#C
                new { UserId = 2, Name = "Jack", ProjectId = 2 });         //#C
            modelBuilder.Entity<User>()                                    //#D
                .OwnsOne(x => x.Address).HasData(                          //#D
                    new {UserId = 1, Street = "Street1", City = "city1"},  //#E
                    new {UserId = 2, Street = "Street2", City = "city2"}); //#E
        }
        /***********************************************************
        #A Seeding is configured via Fluent API
        #B This adds two default projects. Note that you have to provide the primary key
        #C Each Project and a ProjectManager. Note that you set the foreign key of the project they are on
        #D The User class has an Owned type which holds the User's address
        #E These provide the user's addresses. Note that you use the UserId to define which user you are adding data to
         */
    }
    /**********************************************************
     To create a migration I had to
    1. Set the startup project to Test
    2. In PMC type:  Add-Migration Initial -context SeedExampleDbContext -OutputDir SeedExampleMigrations
     **********************************************************/
}