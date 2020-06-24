// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter09Listings.AddViewCommand
{
    public class AddViewCommandDbContext : DbContext
    {
        public AddViewCommandDbContext(
            DbContextOptions<AddViewCommandDbContext> options)
            : base(options)
        { }

        public DbSet<MyEntity> MyEntities { get; set; }

        public DbSet<MyView> MyViews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MyEntity>().ToTable("Entities");
            modelBuilder.Entity<MyView>().ToView("EntityFilterView").HasNoKey();
        }

        /**********************************************************
         To create a migration I had to
        1. Set the startup project to Test
        2. In PMC type:  Add-Migration Initial -context AddViewCommandDbContext -OutputDir Chapter09Listings/AddViewCommand/Migrations
         **********************************************************/

    }
}