// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Reflection;
using BookApp.Domain.Orders;
using BookApp.Domain.Orders.SupportTypes;
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

        protected override void                                        
            OnModelCreating(ModelBuilder modelBuilder)                 
        {
            var utcConverter = new ValueConverter<DateTime, DateTime>(      
                toDb => toDb,                                               
                fromDb =>                                                   
                    DateTime.SpecifyKind(fromDb, DateTimeKind.Utc));        

            foreach (var entityType in modelBuilder.Model.GetEntityTypes()) 
            {
                foreach (var entityProperty in entityType.GetProperties())  
                {
                    if (entityProperty.ClrType == typeof(DateTime)          
                        && entityProperty.Name.EndsWith("Utc"))             
                    {                                                       
                        entityProperty.SetValueConverter(utcConverter);     
                    }                                                       

                    if (entityProperty.ClrType == typeof(decimal)           
                        && entityProperty.Name.Contains("Price"))           
                    {                                                       
                        entityProperty.SetPrecision(9);                     
                        entityProperty.SetScale(2);                         
                    }                                                       

                    if (entityProperty.ClrType == typeof(string)            
                        && entityProperty.Name.EndsWith("Url"))             
                    {                                                       
                        entityProperty.SetIsUnicode(false);                 
                    }                                                       
                }

                if (typeof(IUserId)                             
                    .IsAssignableFrom(entityType.ClrType))      
                {
                    entityType.AddSoftDeleteQueryFilter(        
                        MyQueryFilterTypes.UserId, this);       
                }
            }


            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }

    }
}
