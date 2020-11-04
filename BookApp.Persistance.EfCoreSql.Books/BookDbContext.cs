// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Reflection;
using BookApp.Domain.Books;
using BookApp.Persistence.Common;
using GenericEventRunner.ForDbContext;
using Microsoft.EntityFrameworkCore;

namespace BookApp.Persistence.EfCoreSql.Books
{
    public class BookDbContext                  //#A
        : DbContextWithEvents<BookDbContext>    //#B
    {

        public BookDbContext(
            DbContextOptions<BookDbContext> options, 
            IEventsRunner eventRunner = null) //#C
            : base(options, eventRunner)      //#D
        { }

        /***********************************************************
        #A The BookDbContext handles the Books side of the data 
        #B Instead of inheriting EF Core's DbContext you inherit the class from GenericEventRunner
        #C DI will provide GenericEventRunner's EventRunner. If null then no events used (useful for unit tests)
        #D The constructor of the DbContextWithEvents class needs the EventRunner
         ***********************************************************/

        public DbSet<Book> Books { get; set; }                        
        public DbSet<Author> Authors { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AutoConfigureTypes();
            modelBuilder.AutoConfigureQueryFilters<BookDbContext>(this);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Book>().HasIndex(x => x.LastUpdatedUtc);
            modelBuilder.Entity<Review>().HasIndex(x => x.LastUpdatedUtc);
            modelBuilder.Entity<BookAuthor>().HasIndex(x => x.LastUpdatedUtc);
            modelBuilder.Entity<Author>().HasIndex(x => x.LastUpdatedUtc);

            modelBuilder.RegisterUdfDefinitions();
        }
    }
}
/******************************************************************************
* NOTES ON MIGRATION:
*
* BookApp.UI has two application DbContexts, BookDbContext and OrderDbContest
* Each has its own project, migrations and migration history table
* You need to build a migration from the DbContext's project (see below)
*
* NOTE: The EF Core commands give a error, but it does create the migration
* 
* see https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/migrations?tabs=visual-studio
* 
* The following NuGet libraries must be loaded
* 1. Add to BookApp: "Microsoft.EntityFrameworkCore.Tools"
* 2. Add to DataLayer: "Microsoft.EntityFrameworkCore.SqlServer" (or another database provider)
* 
* 2. Using Package Manager Console commands
* The steps are:
* a) Make sure the default project is BookApp.Persistence.EfCoreSql.Books
* b) Use the PMC command
*    Add-Migration NameForMigration -Context BookDbContext
* c) Use PMC command
*    Update-database (or migrate on startup)
*    
* If you want to start afresh then:
* a) Delete the current database
* b) Delete all the class in the Migration directory
* c) follow the steps to add a migration
******************************************************************************/
