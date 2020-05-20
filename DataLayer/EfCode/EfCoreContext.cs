// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Reflection;
using DataLayer.EfClasses;
using DataLayer.EfCode.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode
{
    public class EfCoreContext : DbContext
    {
        private readonly Guid _userId;                                //#A                               
                                                                      
        public EfCoreContext(DbContextOptions<EfCoreContext> options, //#B  
            IUserIdService userIdService = null)                      //#B  
            : base(options)                                           //#B
        {                                                             //#B
            _userId = userIdService?.GetUserId()                      //#B  
                       ?? new ReplacementUserIdService().GetUserId(); //#B  
        }                                                             //#B
                                                                      //#B
        public DbSet<Book> Books { get; set; }                        //#C
        public DbSet<Author> Authors { get; set; }                    //#C
        public DbSet<PriceOffer> PriceOffers { get; set; }            //#C
        public DbSet<Order> Orders { get; set; }                      //#C
                                                                      
        protected override void                                       //#D
            OnModelCreating(ModelBuilder modelBuilder)                //#D
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            //modelBuilder.ApplyConfiguration(new BookConfig());        //#E
            //modelBuilder.ApplyConfiguration(new BookAuthorConfig());  //#E
            //modelBuilder.ApplyConfiguration(new PriceOfferConfig());  //#E
            //modelBuilder.ApplyConfiguration(new LineItemConfig());    //#E
                                                                      
            modelBuilder.Entity<Order>()                              //#F
                .HasQueryFilter(x => x.CustomerId == _userId);        //#F
        }
    }
    /***************************************************************
    #A This is the UserId of the user that has bought some books
    #B As well as setting up the DbContext options this also obtains the current UserId
    #C These are the entity classes that your code will access
    #D This is the method in which runs your fluent API commands
    #E These run each of the separate configurations for each entity class that needs configuration
    #F This Query Filter is in the OnModelCreating so that it can pick up the current UserId
     **********************************************************/
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
*    Update-database (or migrate on startup)
*    
* If you want to start afresh then:
* a) Delete the current database
* b) Delete all the class in the Migration directory
* c) follow the steps to add a migration
******************************************************************************/
