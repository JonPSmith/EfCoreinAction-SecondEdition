// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch02_CreateDbContext
    {
        public Ch02_CreateDbContext()
        {
            const string connection =
                "Data Source=(localdb)\\mssqllocaldb;" +
                "Database=EfCoreInActionDb.Chapter02;" +
                "Integrated Security=True;";
            var optionsBuilder =
                new DbContextOptionsBuilder
                    <EfCoreContext>();

            optionsBuilder.UseSqlServer(connection);
            var options = optionsBuilder.Options;

            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
            }
        }

        [Fact]
        public void TestCreateAOk()
        {
            //SETUP
            const string connection =
                "Data Source=(localdb)\\mssqllocaldb;" + //#A
                "Database=EfCoreInActionDb.Chapter02;" + //#A
                "Integrated Security=True;"; //#A
            var optionsBuilder = //#B
                new DbContextOptionsBuilder //#B
                    <EfCoreContext>(); //#B

            optionsBuilder.UseSqlServer(connection); //#C
            var options = optionsBuilder.Options;

            using (var context = new EfCoreContext(options)) //#D
            {
                var bookCount = context.Books.Count(); //#E
                //... etc.
                /******************************************************************
                #A This is the "connection string". Its format is dictated by the sort of database provider and hosting you are using.
                #B I need a EF Core DbContextOptionsBuilder<> instance to be able to set the options we need.
                #C I am accessing a SQL Server database so use the UseSqlServer method from the Microsoft.EntityFrameworkCore.SqlServer library, need the connection string.
                #D This creates the all-important EfCoreContext using the options we have set up. Note that I use a 'using' statement as the DbContext is disposable, i.e. it should be 'disposed' once you have finished your data access 
                #E This code uses the DbContext to find out how many books are in the database.
                * ****************************************************************/

                //VERIFY
            }
        }
    }
}