// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Reflection;
using DataLayer.EfClasses;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataLayer.EfCode
{
    public class EfCoreContext : DbContext, IUserId                   //#A
    {
        public Guid UserId { get; private set; }                      //#B

        public EfCoreContext(DbContextOptions<EfCoreContext> options, //#C  
            IUserIdService userIdService = null)                      //#C  
            : base(options)                                           //#C
        {                                                             //#C
            UserId = userIdService?.GetUserId()                       //#C  
                     ?? new ReplacementUserIdService().GetUserId();   //#C  
        }

        //#B
        public DbSet<Book> Books { get; set; }                        //#C
        public DbSet<Author> Authors { get; set; }                    //#C
        public DbSet<PriceOffer> PriceOffers { get; set; }            //#C
        public DbSet<Tag> Tags { get; set; }                          //#C

        public DbSet<Order> Orders { get; set; }                      //#C

        protected override void                                       //#D //#A
            OnModelCreating(ModelBuilder modelBuilder)                //#D //#A
        {
            var utcConverter = new ValueConverter<DateTime, DateTime>(      //#B
                toDb => toDb,                                               //#B
                fromDb =>                                                   //#B
                    DateTime.SpecifyKind(fromDb, DateTimeKind.Utc));        //#B

            foreach (var entityType in modelBuilder.Model.GetEntityTypes()) //#G
            {
                foreach (var entityProperty in entityType.GetProperties())  //#D
                {
                    if (entityProperty.ClrType == typeof(DateTime)          //#E
                        && entityProperty.Name.EndsWith("Utc"))             //#E
                    {                                                       //#E
                        entityProperty.SetValueConverter(utcConverter);     //#E
                    }                                                       //#E

                    if (entityProperty.ClrType == typeof(decimal)           //#F
                        && entityProperty.Name.Contains("Price"))           //#F
                    {                                                       //#F
                        entityProperty.SetPrecision(9);                     //#F
                        entityProperty.SetScale(2);                         //#F
                    }                                                       //#F

                    if (entityProperty.ClrType == typeof(string)            //#G
                        && entityProperty.Name.EndsWith("Url"))             //#G
                    {                                                       //#G
                        entityProperty.SetIsUnicode(false);                 //#G
                    }                                                       //#G
                }

                if (typeof(ISoftDelete)                         //#F
                    .IsAssignableFrom(entityType.ClrType))      //#F
                {
                    entityType.AddSoftDeleteQueryFilter(        //#G
                        MyQueryFilterTypes.SoftDelete);         //#G
                }
                if (typeof(IUserId)                             //#H
                    .IsAssignableFrom(entityType.ClrType))      //#H
                {
                    entityType.AddSoftDeleteQueryFilter(        //#I
                        MyQueryFilterTypes.UserId, this);       //#I
                }
            }
            /**********************************************************************
            //Listing 7.13 Automatically apply a Value Converter to a DateTime property ending in Utc
            #A The Fluent API commands are applied in the OnModelCreating method
            #B This defines a Value Converter to set the UTC setting to the returned DateTime
            #C This loops through all the classes that EF Core has currently found mapped to the database
            #D This loops through all the properties in an entity class that are mapped to the database
            #E This adds the UTC Value Converter to properties of type DateTime and Name ending in "Utc"
            #F This sets the precision/scale to properties of type decimal and the Name contains in "Price"
            #G This sets the string to ASCII on properties of type string and the Name ending in "Url"
             ********************************************************************/

            /************************************************************************
             //Listing 7.15 Adding code to the DbContext to automate setting up Query Filters
            #A Adding the IUserId to the DbContext means we can pass the DbContext to the UerId query filter 
            #B Because the IUserId interface to the DbContext you now have a UserId property to access
            #C This set up the UserId. If the userIdService is null, or it returns null for the UserId we set a replacement UserId 
            #D The automate code goes in the OnModelCreating method
            #E This loops through all the classes that EF Core has currently found mapped to the database
            #F If the class inherits the ISoftDelete interface, then is needs the SoftDelete Query Filter
            #G This adds a Query Filter to this class, with a query suitable for SoftDelete
            #H If the class inherits the IUserId interface, then is needs the IUserId Query Filter
            #I This adds the UserId Query Filter to this class - passing 'this' allows access to the current UserId
             ************************************************************************/
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            //modelBuilder.ApplyConfiguration(new BookConfig());        //#E
            //modelBuilder.ApplyConfiguration(new BookAuthorConfig());  //#E
            //modelBuilder.ApplyConfiguration(new PriceOfferConfig());  //#E
            //modelBuilder.ApplyConfiguration(new LineItemConfig());    //#E

            //modelBuilder.Entity<Order>()                              //#F
            //    .HasQueryFilter(x => x.UserId == this.UserId);        //#F

            //This is a check on detecting the database type (Sqlite can't filter on a decimal
            if (Database.IsSqlite())              //#A 
            {                                     
                modelBuilder.Entity<Book>()       //#B
                    .Property(e => e.Price)       //#B
                    .HasConversion<double>();     //#B
                modelBuilder.Entity<PriceOffer>() //#B
                    .Property(e => e.NewPrice)    //#B
                    .HasConversion<double>();     //#B
            }
            
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
*    Add-Migration NameForMigration -Project DataLayer -Context EfCoreContext
* c) Use PMC command
*    Update-database (or migrate on startup)
*    
* If you want to start afresh then:
* a) Delete the current database
* b) Delete all the class in the Migration directory
* c) follow the steps to add a migration
******************************************************************************/
