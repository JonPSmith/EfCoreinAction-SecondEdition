// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter08Listings.EFCode.Configurations;
using Person = Test.Chapter07Listings.Person;

namespace Test.Chapter08Listings.EFCode
{
    public class Chapter08DbContext : DbContext
    {
        public Chapter08DbContext(
            DbContextOptions<Chapter08DbContext> options)
            : base(options)
        { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeShortFk> EmployeeShortFks { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<LibraryBook> LibraryBooks { get; set; }

        //One-to-One versions
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<TicketOption1> TicketOption1s { get; set; }
        public DbSet<TicketOption2> TicketOption2s { get; set; }
        public DbSet<TicketOption3> TicketOption3s { get; set; }

        //Table-per-hierarchy
        public DbSet<Payment> Payments { get; set; } //#A
        public DbSet<SoldIt> SoldThings { get; set; } //#B

        //Backing fields on relationships
        public DbSet<Ch08Book> Books { get; set; }
        public DbSet<PriceOffer> PriceOffers { get; set; }

        //delete behavior
        public DbSet<DeletePrincipal> DeletePrincipals { get; set; }

        protected override void OnModelCreating
            (ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AttendeeConfig());
            modelBuilder.ApplyConfiguration(new PersonConfig());
            modelBuilder.ApplyConfiguration(new EmployeeShortFkConfig());
            modelBuilder.ApplyConfiguration(new Ch07BookConfig());
            modelBuilder.ApplyConfiguration(new DeletePrincipalConfig());
            modelBuilder.ApplyConfiguration(new PaymentConfig()); //#C
        }
    }
    /**TPH**************************************************
    #A This defines the property through which I can access all the payments, both PaymentCash and PaymentCard
    #B This is the list of sold items, with a required link to a Payment
    #C I call the configureration code for the payment TPH via its extension method, Configure
     * ******************************************************/
}