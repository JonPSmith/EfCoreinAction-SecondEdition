// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter12Listings.BusinessLogic;
using Test.Chapter12Listings.IntegrationEventEfClasses;

namespace Test.Chapter12Listings.EfCode
{
    public class IntegrationEventDbContext : DbContext
    {
        private readonly IWarehouseEventHandler                     //#A
            _warehouseEventHandler;                                 //#A

        public IntegrationEventDbContext(                         
            DbContextOptions<IntegrationEventDbContext> options, 
            IWarehouseEventHandler warehouseEventHandler)           //#B
            : base(options)
        {
            _warehouseEventHandler = warehouseEventHandler;         //#B
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }

        public override int SaveChanges                             //#C
            (bool acceptAllChangesOnSuccess)                        //#C
        {
            if (!_warehouseEventHandler.NeedsCallToWarehouse(this)) //#D
                return                                              //#D
                    base.SaveChanges(acceptAllChangesOnSuccess);    //#D

            using(var transaction = Database.BeginTransaction())    //#E
            {
                var result =                                        //#F
                    base.SaveChanges(acceptAllChangesOnSuccess);    //#F

                var errors = _warehouseEventHandler                 //#G
                    .AllocateOrderAndDispatch();                    //#G

                if (errors.Any())                                   //#H
                {                                                   //#H
                    throw new OutOfStockException(                  //#H
                        string.Join('.', errors));                  //#H
                }                                                   //#H

                transaction.Commit();                               //#I
                return result;                                      //#J
            }
        }

        //You should also override SaveChangesAsync

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                new {ProductCode = "B1x2Red", Name = "Brick 1x2, Red"},
                new {ProductCode = "B1x4Red", Name = "Brick 1x4, Red"},
                new {ProductCode = "B1x6Red", Name = "Brick 1x6, Red"},
                new {ProductCode = "B1x8Red", Name = "Brick 1x8, Red"},
                new {ProductCode = "B2x2Red", Name = "Brick 2x2, Red"},
                new {ProductCode = "B2x4Red", Name = "Brick 2x4, Red"},
                new {ProductCode = "B2x6Red", Name = "Brick 2x6, Red"},
                new {ProductCode = "B2x8Red", Name = "Brick 2x8, Red"},

                new {ProductCode = "B1x2Blue", Name = "Brick 1x2, Blue"},
                new {ProductCode = "B1x4Blue", Name = "Brick 1x4, Blue"},
                new {ProductCode = "B1x6Blue", Name = "Brick 1x6, Blue"},
                new {ProductCode = "B1x8Blue", Name = "Brick 1x8, Blue"},
                new {ProductCode = "B2x2Blue", Name = "Brick 2x2, Blue"},
                new {ProductCode = "B2x4Blue", Name = "Brick 2x4, Blue"},
                new {ProductCode = "B2x6Blue", Name = "Brick 2x6, Blue"},
                new {ProductCode = "B2x8Blue", Name = "Brick 2x8, Blue"}
            );
        }
    }
    /***************************************************************
    #A This holds the instance of the warehouse event handler 
    #B You inject the warehouse event handler via DI
    #C You override SaveChanges to include the warehouse event handler
    #D If the event handler doesn't detect an event it does a normal SaveChanges
    #E There is an integration event so a transaction is opened
    #F It first calls the base SaveChange to save the Order
    #G This calls the warehouse event handler that communicates with the warehouse
    #H If the warehouse returned errors, then it throws a OutOfStockException
    #I If there were no errors the Order is committed to the database
    #J Finally we return the result of the SaveChanges
     ***************************************************************/
}