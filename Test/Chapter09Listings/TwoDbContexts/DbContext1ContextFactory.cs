// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Test.Chapter09Listings.TwoDbContexts
{
    public class DbContext1ContextFactory : IDesignTimeDbContextFactory<DbContext1>  
    {
        public DbContext1 CreateDbContext(string[] args) 
        {
            var optionsBuilder =                       
                new DbContextOptionsBuilder<DbContext1>(); 
            optionsBuilder.UseSqlServer("dummy",
                x => x.MigrationsHistoryTable($"__{nameof(DbContext1)}")); 

            return new DbContext1(optionsBuilder.Options);
        }
    }
}