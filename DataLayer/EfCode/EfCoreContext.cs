// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode
{
    public class EfCoreContext : DbContext
    {
        private readonly Guid _userId;                                     //#A

        public EfCoreContext(DbContextOptions<EfCoreContext> options,      //#B
            IUserIdService userIdService = null)                           //#C
            : base(options)
        {
            _userId = userIdService?.GetUserId()                           //#D
                       ?? new ReplacementUserIdService().GetUserId();      //#D
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<PriceOffer> PriceOffers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) //#E
        {
            modelBuilder.Entity<BookAuthor>() 
                .HasKey(x => new {x.BookId, x.AuthorId});

            modelBuilder.Entity<LineItem>()
                .HasOne(p => p.ChosenBook) 
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>()                                    //#F
                .HasQueryFilter(p => !p.SoftDeleted);                      //#F
                                                            
            modelBuilder.Entity<Order>()                                   //#G
                .HasQueryFilter(x => x.CustomerId == _userId);             //#G
        } 
    }
}
/*********************************************************
#A This property holds the UserId to filter the Order entity class by
#B This is the normal options for setting up the application's DbContext
#C This is the UserIdService. Note that I make this an optional parameter - that makes it much easier to use in unit tests that don't use the query filter
#D This sets the UserId. Note that the UserIdService was null you use a simple replacement version that returns the default Guid.Empty value.
#E This is the method where you configure EF Core, and it's the place where you put your query filters in
#F This is the soft delete query filter
#G And this is the Order query filter which matches the current UserId obtains from the cookie basket with the CustomerId in the Order entity class
* ******************************************************/

/******************************************************************************
* NOTES ON MIGRATION:
* 
* see https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/migrations?tabs=visual-studio
* 
* The following NuGet libraries must be loaded
* 1. Add to BookApp: "Microsoft.EntityFrameworkCore.Tools"
* 2. Add to DataLayer: "Microsoft.EntityFrameworkCore.SqlServer" (or another database provider)
* 
* 2. Using Package Manager Console commands
* The steps are:
* a) Make sure the default project is BookApp
* b) Use the PMC command
*    Add-Migration NameForMigration -Project DataLayer
* c) Use PMC command
*    Update-database (or migrate on startup)
*    
* If you want to start afresh then:
* a) Delete the current database
* b) Delete all the class in the Migration directory
* c) follow the steps to add a migration
******************************************************************************/
