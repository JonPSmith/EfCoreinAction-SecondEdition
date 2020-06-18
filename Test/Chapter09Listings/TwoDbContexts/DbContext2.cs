// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter09Listings.TwoDbContexts
{
    public class DbContext2 : DbContext
    {
        public DbContext2(
            DbContextOptions<DbContext2> options)
            : base(options)
        { }

        public DbSet<OnlyIn2> OnlyIn2s { get; set; }
        public DbSet<Shared> Shared { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shared>()
                .ToView("Shared");
        }
    }
    //Set Test as startup project and then enter PMC command to create the migration
    //Add-Migration Initial -context DbContext2 -OutputDir Chapter09Listings/TwoDbContexts/Migration2
}