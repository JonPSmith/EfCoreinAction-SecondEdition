// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Test.Chapter09Listings.TwoDbContexts
{
    public class DbContext2ContextFactory : IDesignTimeDbContextFactory<DbContext2>  
    {
        public DbContext2 CreateDbContext(string[] args) 
        {
            var optionsBuilder =                       
                new DbContextOptionsBuilder<DbContext2>();
            optionsBuilder.UseSqlServer("dummy",
                x => x.MigrationsHistoryTable($"__{nameof(DbContext2)}")); 

            return new DbContext2(optionsBuilder.Options);
        }
    }
}