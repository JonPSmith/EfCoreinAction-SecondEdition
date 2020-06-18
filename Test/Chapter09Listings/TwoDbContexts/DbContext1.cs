// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter09Listings.TwoDbContexts
{
    public class DbContext1 : DbContext
    {
        public DbContext1(DbContextOptions<DbContext1> options)
            : base(options)
        { }

        public DbSet<OnlyIn1> OnlyIn1s { get; set; }
        public DbSet<Shared> Shared { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shared>()
                .ToTable("Shared");
        }
    }
    //Set Test as startup project and then enter PMC command to create the migration
    //Add-Migration Initial -context DbContext1 -OutputDir Chapter09Listings/TwoDbContexts/Migration1
}