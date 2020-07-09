// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter09Listings.FiveStepMigration
{
    public class App2DbContext : DbContext
    {
        public DbSet<UserPart2> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public DbSet<ReadOnlyUserWithAddress> ReadOnlyUserWithAddresses { get; set; }

        public App2DbContext(DbContextOptions<App2DbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReadOnlyUserWithAddress>().ToView("GetUserWithAddress");
        }
    }
}