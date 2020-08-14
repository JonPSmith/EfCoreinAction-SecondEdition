// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Reflection;
using BookApp.Domain.Orders;
using BookApp.Domain.Orders.SupportTypes;
using BookApp.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BookApp.Persistence.NormalSql.Orders
{
    public class OrderDbContext : DbContext, IUserId                   
    {
        public Guid UserId { get; private set; }                      

        public OrderDbContext(DbContextOptions<OrderDbContext> options,   
            IUserIdService userIdService = null)                        
            : base(options)                                           
        {                                                             
            UserId = userIdService?.GetUserId()                         
                     ?? new ReplacementUserIdService().GetUserId();     
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<BookView> BookViews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)                 
        {
            modelBuilder.AutoConfigureTypes();
            modelBuilder.AutoConfigureQueryFilters<OrderDbContext>(this);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

    }
}
