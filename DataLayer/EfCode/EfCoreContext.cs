// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode
{
    public class EfCoreContext : DbContext
    {
        public EfCoreContext(DbContextOptions<EfCoreContext> options)
            : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<PriceOffer> PriceOffers { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<BookAuthor>() 
                .HasKey(x => new {x.BookId, x.AuthorId});

            modelBuilder.Entity<LineItem>()
                .HasOne(p => p.ChosenBook) 
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>()
                .HasQueryFilter(p => !p.SoftDeleted);
        } 
    }

    /*********************************************************
    #A The three properties link to the database tables with the same name
    #B This constructor is how the ASP.NET creates an instance of EfCoreContext 
    #C I need to tell EF Core about the Many-to-Many table keys. I explain this in detail in chapters 5 and 6
    * ******************************************************/
}

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
*    Update-database
*    
* If you want to start afresh then:
* a) Delete the current database
* b) Delete all the class in the Migration directory
* c) follow the steps to add a migration
******************************************************************************/
