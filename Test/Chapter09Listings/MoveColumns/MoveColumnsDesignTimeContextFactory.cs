// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Test.Chapter09Listings.MoveColumns
{
    public class MoveColumnsDesignTimeContextFactory : IDesignTimeDbContextFactory<MoveColumnsDbContext>  
    {
        public MoveColumnsDbContext CreateDbContext(string[] args) 
        {
            var optionsBuilder =                       
                new DbContextOptionsBuilder<MoveColumnsDbContext>();
            optionsBuilder.UseSqlServer("dummy");

            return new MoveColumnsDbContext(optionsBuilder.Options);
        }
    }
}