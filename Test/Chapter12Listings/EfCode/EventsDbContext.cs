// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter12Listings.EfClasses;
using Test.Chapter12Listings.EventInterfacesEtc;
using Test.Chapter12Listings.EventRunnerCode;

namespace Test.Chapter12Listings.EfCode
{
    public class EventsDbContext : DbContext
    {
        private readonly IEventRunner _eventRunner;

        public EventsDbContext(
            DbContextOptions<EventsDbContext> options,
            IEventRunner eventRunner = null)
            : base(options)
        {
            _eventRunner = eventRunner;
        }

        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<SalesTaxInfo> SalesTaxes { get; set; }


        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            _eventRunner?.RunEvents(this);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SalesTaxInfo>().HasData(
                new {State = "006", SalesTaxPercent = 0.06},
                new {State = "005", SalesTaxPercent = 0.05},
                new {State = "009", SalesTaxPercent = 0.09},
                new {State = "004", SalesTaxPercent = 0.04});
            modelBuilder.Entity<Location>().HasData(
                new { LocationId = 1, Name = "Place1", State = "005" },
                new { LocationId = 2, Name = "Place2", State = "005" },
                new { LocationId = 3, Name = "Place3", State = "005" });
        }
    }
}