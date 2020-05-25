// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using DataLayer.EfClasses;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
        } //#B

        //#B
        public DbSet<Book> Books { get; set; }                        //#C
        public DbSet<Author> Authors { get; set; }                    //#C
        public DbSet<PriceOffer> PriceOffers { get; set; }            //#C
        public DbSet<Order> Orders { get; set; }                      //#C

        protected override void                                       //#D //#A
            OnModelCreating(ModelBuilder modelBuilder)                //#D //#A
        {
            //see https://github.com/aspnet/EntityFrameworkCore/issues/4711#issuecomment-535288442 for example with nullable DateTime
            var utcConverter = new ValueConverter<DateTime, DateTime>(      //#B
                toDb => toDb,                                               //#B
                fromDb =>                                                   //#B
                    DateTime.SpecifyKind(fromDb, DateTimeKind.Utc));        //#B

            foreach (var entityType in modelBuilder.Model.GetEntityTypes()) //#C
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
                        entityProperty.SetIsUnicode(true);                  //#G
                    }                                                       //#G
                }

                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    AddQueryFilterAutomatically(entityType, MyQueryFilterTypes.SoftDelete);
                }
                if (typeof(IUserId).IsAssignableFrom(entityType.ClrType))
                {
                    AddQueryFilterAutomatically(entityType, MyQueryFilterTypes.UserId);
                }
            }
            /**********************************************************************
            #A The Fluent API commands are applied in the OnModelCreating method
            #B This defines a Value Converter to set the UTC setting to the returned DateTime
            #C This will loop through all the classes that EF Core has currently found mapped to the database
            #D This will loop through all the properties in an entity class that are mapped to the database
            #E This adds the UTC Value Converter to properties of type DateTime and Name ending in "Utc"
            #F This sets the precision/scale to properties of type decimal and the Name contains in "Price"
            #G This sets the string to ASCII on properties of type string and the Name ending in "Url"
             ********************************************************************/

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            //modelBuilder.ApplyConfiguration(new BookConfig());        //#E
            //modelBuilder.ApplyConfiguration(new BookAuthorConfig());  //#E
            //modelBuilder.ApplyConfiguration(new PriceOfferConfig());  //#E
            //modelBuilder.ApplyConfiguration(new LineItemConfig());    //#E
                                                                      
            modelBuilder.Entity<Order>()                              //#F
                .HasQueryFilter(x => x.UserId == _userId);        //#F
        }



        private enum MyQueryFilterTypes { SoftDelete, UserId }

        private void AddQueryFilterAutomatically(IMutableEntityType entityData, 
            MyQueryFilterTypes queryFilterType)
        {
            var methodName = $"Get{queryFilterType}Filter";
            var methodToCall = this.GetType().GetMethod(methodName, 
                BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(entityData.ClrType);
            var filter = methodToCall.Invoke(this, new object[]{});
            entityData.SetQueryFilter((LambdaExpression)filter);
        }
        private LambdaExpression GetUserIdFilter<TEntity>()
            where TEntity : class, IUserId
        {
            Expression<Func<TEntity, bool>> filter = x => x.UserId == _userId;
            return filter;
        }

        private LambdaExpression GetSoftDeleteFilter<TEntity>()
            where TEntity : class, ISoftDelete
        {
            Expression<Func<TEntity, bool>> filter = x => !x.SoftDeleted;
            return filter;
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
