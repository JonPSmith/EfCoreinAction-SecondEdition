// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter10Listings.EfClasses;

namespace Test.Chapter10Listings.EfCode
{
    public class ConcurrencyDbContext : DbContext
    {
        public ConcurrencyDbContext(
            DbContextOptions<ConcurrencyDbContext> options)      
                : base(options) { }

        public DbSet<ConcurrencyBook> Books { get; set; }
        public DbSet<ConcurrencyAuthor> Authors { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void
            OnModelCreating(ModelBuilder modelBuilder) //#A
        {
            modelBuilder.Entity<ConcurrencyBook>()//#B
                .Property(p => p.PublishedOn)    //#B
                .IsConcurrencyToken();           //#B

            modelBuilder.Entity<ConcurrencyAuthor>() //#C
                .Property(p => p.ChangeCheck) //#C
                .IsRowVersion(); //#C

        }

        /****************************************************
        #A The OnModelCreating method is where I place the configuration of the concurrecy detection
        #B I define the property PublishedOn as a concurrency token, which means EF Core checks it hasn't changed when write out an update
        #C I define an extra property called ChangeCheck, that will be changed every time the row is created/updated. EF Core checks it hasn't changed when write out an update
            * ************************************************/
    }
}