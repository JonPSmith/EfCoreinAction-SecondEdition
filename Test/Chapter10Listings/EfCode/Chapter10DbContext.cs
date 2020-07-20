// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter10Listings.EfClasses;
using Test.Chapter10Listings.EfCode.Configuration;

namespace Test.Chapter10Listings.EfCode
{
    public class Chapter10DbContext : DbContext
    {
        public DbSet<DefaultTest> Defaults { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<MyClass> MyClasses { get; set; }
        //standard localdb is 2014, not 2016, so in-memory is not supported
        //public DbSet<InMemoryTest> InMemory { get; set; }

        public Chapter10DbContext(
            DbContextOptions<Chapter10DbContext> options)
            : base(options)
        { }

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DefaultTest>()
                .Configure(new OrderIdValueGenerator());
            modelBuilder.ConfigureOrder();
            modelBuilder.ApplyConfiguration(new PersonConfig());
            //standard localdb is 2014, not 2016, so in-memory is not supported
            //modelBuilder.ApplyConfiguration(new InMemoryConfig());

        }
    }
}
