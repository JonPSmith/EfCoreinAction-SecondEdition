// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Test.Chapter09Listings.SeedExample
{
    public class SeedDesignTimeContextFactory : IDesignTimeDbContextFactory<SeedExampleDbContext>  
    {
        public SeedExampleDbContext CreateDbContext(string[] args) 
        {
            var optionsBuilder =                       
                new DbContextOptionsBuilder<SeedExampleDbContext>(); 
            optionsBuilder.UseSqlite("Filename=:memory:"); 

            return new SeedExampleDbContext(optionsBuilder.Options);
        }
    }
}