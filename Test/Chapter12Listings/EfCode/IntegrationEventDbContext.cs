// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter12Listings.BusinessLogic;
using Test.Chapter12Listings.IntegrationEventEfClasses;

namespace Test.Chapter12Listings.EfCode
{
    public class IntegrationEventDbContext : DbContext
    {
        private readonly IWarehouseService _warehouseService;

        public IntegrationEventDbContext(                         
            DbContextOptions<IntegrationEventDbContext> options, 
            IWarehouseService warehouseService = null)  
            : base(options)
        {
            _warehouseService = warehouseService;
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }

        public override int SaveChanges
            (bool acceptAllChangesOnSuccess)
        {
            var newOrders = ChangeTracker.Entries<Order>()
                .Where(x => x.State == EntityState.Added)
                .Select(x => x.Entity)
                .ToList();
            if (!newOrders.Any() || _warehouseService == null)
                return base.SaveChanges(acceptAllChangesOnSuccess);

            using(var transaction = Database.BeginTransaction())
            {
                var result = base.SaveChanges(acceptAllChangesOnSuccess);
                _warehouseService.CheckProductsInStock(newOrders);
                transaction.Commit();
                return result;
            }
        }

        //You should also override SaveChangesAsync

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                new { ProductCode = "B1x2Red", Name = "Brick 1x2, Red" },
                new { ProductCode = "B1x4Red", Name = "Brick 1x4, Red" },
                new { ProductCode = "B1x6Red", Name = "Brick 1x6, Red" },
                new { ProductCode = "B1x8Red", Name = "Brick 1x8, Red" },
                new { ProductCode = "B2x2Red", Name = "Brick 2x2, Red" },
                new { ProductCode = "B2x4Red", Name = "Brick 2x4, Red" },
                new { ProductCode = "B2x6Red", Name = "Brick 2x6, Red" },
                new { ProductCode = "B2x8Red", Name = "Brick 2x8, Red" },

                new { ProductCode = "B1x2Blue", Name = "Brick 1x2, Blue" },
                new { ProductCode = "B1x4Blue", Name = "Brick 1x4, Blue" },
                new { ProductCode = "B1x6Blue", Name = "Brick 1x6, Blue" },
                new { ProductCode = "B1x8Blue", Name = "Brick 1x8, Blue" },
                new { ProductCode = "B2x2Blue", Name = "Brick 2x2, Blue" },
                new { ProductCode = "B2x4Blue", Name = "Brick 2x4, Blue" },
                new { ProductCode = "B2x6Blue", Name = "Brick 2x6, Blue" },
                new { ProductCode = "B2x8Blue", Name = "Brick 2x8, Blue" }
                );

        }
    }
}