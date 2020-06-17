// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataLayer.EfCode
{
    public class DesignTimeContextFactory                     //#A
        : IDesignTimeDbContextFactory<EfCoreContext>          //#B
    {
        private const string connectionString =               //#C
            "Server=(localdb)\\mssqllocaldb;Database=EfCoreInActionDb2-Part2;Trusted_Connection=True;MultipleActiveResultSets=true";

        public EfCoreContext CreateDbContext(string[] args)   //#D
        {
            var optionsBuilder =                              //#E
                new DbContextOptionsBuilder<EfCoreContext>(); //#E
            optionsBuilder.UseSqlServer(connectionString);    //#E

            return new EfCoreContext(optionsBuilder.Options); //#F
        }
    }
    /*********************************************************************
    #A The class is used by EF Core tools to obtain an instance of the DbContext
    #B This interface defines a way that the EF Core tools find and create this class
    #C You need to provide a connection string to your local database
    #D This is the method the interface requires. It returns a valid instance of the DbContext
    #E You use the normal commands for setting up the database provider you are using
    #F It returns the DbContext for the EF Core tools to use
     *******************************************************************/
}