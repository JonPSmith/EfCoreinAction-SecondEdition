// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using Test.Chapter17Listings;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.Chapter17Tests
{
    public class TestCreateInstance
    {
        [Fact]
        public void TestExampleSetupViaConstructorOk()
        {
            //SETUP
            const string connectionString //#A
                = "Server=(localdb)\\mssqllocaldb;Database=EfCore.TestSupport-Test;Trusted_Connection=True";
            var builder = new                             //#B
                DbContextOptionsBuilder<BookDbContext>(); //#B
            builder.UseSqlServer(connectionString);       //#C
            var options = builder.Options;                //#D
            using (var context = new BookDbContext(options)) //#E
            {
                //VERIFY
                context.Database.GetDbConnection().ConnectionString.ShouldEqual(connectionString);
            }
            /********************************************************************
            #A This holds the connection string for the SQL Server database
            #B We need to create DbContextOptionsBuilder<T> class to build the options
            #C Here I define that I want to use the SQL Server database provider
            #D I then build the final DbContextOptions<EfCoreContext> options that the application's DbContext needs
            #E This then allows me to create an instance for my unit tests
             * ******************************************************************/
        }

        [Fact]
        public void TestExampleOnConfiguringNormalOk()
        {
            //SETUP
            const string expectedConnectionString //#A
                = "Server=(localdb)\\mssqllocaldb;Database=EfCore.TestSupport-Test-OnConfiguring;Trusted_Connection=True";
            //ATTEMPT
            using (var context = new DbContextOnConfiguring()) //#B
            {
                //VERIFY
                context.Database.GetDbConnection().ConnectionString.ShouldEqual(expectedConnectionString);
            }
        }

        [Fact]
        public void TestExampleSetupViaOnConfiguringOk()
        {
            //SETUP
            const string connectionString //#A
                = "Server=(localdb)\\mssqllocaldb;Database=EfCore.TestSupport-Test;Trusted_Connection=True";
            var builder = new                             //#B
                DbContextOptionsBuilder<DbContextOnConfiguring>();//#B
            builder.UseSqlServer(connectionString);       //#C
            var options = builder.Options;                //#D
            //ATTEMPT
            using (var context = new DbContextOnConfiguring //#B
                (options))                         //#B
            {
                //VERIFY
                context.Database.GetDbConnection().ConnectionString.ShouldEqual(connectionString);
            }
            /********************************************************************
            #A This holds the connection string for the database to be used for the unit test
            #B I set up the options I want to use
            #C I then provide the options to the DbContext via is new, one-parameter constructor
             * ******************************************************************/
        }
    }

}