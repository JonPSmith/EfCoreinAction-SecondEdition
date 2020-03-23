// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode
{
    public class EfCoreContext : DbContext
    {
        public DbSet<Book> Books { get; set; }            //#A
        public DbSet<Author> Authors { get; set; }        //#A
        public DbSet<PriceOffer> PriceOffers { get; set; }//#A

        public EfCoreContext(                             //#B
            DbContextOptions<EfCoreContext> options)      //#B
            : base(options) {}                            //#B

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)    //#C
        {                                                 //#C
            modelBuilder.Entity<BookAuthor>()             //#C
                .HasKey(x => new {x.BookId, x.AuthorId}); //#C
        }                                                 //#C
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
* see https://docs.microsoft.com/en-us/ef/core/get-started/aspnetcore/new-db
* 
* Add to EfCoreInAction the following NuGet libraries
* 1. "Microsoft.EntityFrameworkCore.Tools"  AND MOVE TO tools part of project
*    Note: You can move the Microsoft.EntityFrameworkCore.Tools pckage to the tools part of project. 
* 
* 2. Using Package Manager Console commands
* The steps are:
* a) Add a second param to the AddDbContext command in startup which is
*    b => b.MigrationsAssembly("DataLayer")
* b) Use the PMC command
*    Add-Migration Chapter02 -Project DataLayer -StartupProject EfCoreInAction
* c) Use PMC command
*    Update-database -Project DataLayer -StartupProject EfCoreInAction
*    
* If you want to start afreash then:
* a) Delete the current database
* b) Delete all the class in the Migration directory
* c) follow the steps to add a migration
******************************************************************************/
