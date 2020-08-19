// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookApp.Persistence.EfCoreSql.Orders
{
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<OrderDbContext>          
    {
        private const string connectionString =               
            "Server=(localdb)\\mssqllocaldb;Database=EfCoreInActionDb2-Part3;Trusted_Connection=True;MultipleActiveResultSets=true";

        public OrderDbContext CreateDbContext(string[] args)   
        {
            var optionsBuilder =                              
                new DbContextOptionsBuilder<OrderDbContext>(); 
            optionsBuilder.UseSqlServer(connectionString, dbOptions =>
                dbOptions.MigrationsHistoryTable("OrderMigrationHistoryName"));    

            return new OrderDbContext(optionsBuilder.Options); 
        }
    }
}