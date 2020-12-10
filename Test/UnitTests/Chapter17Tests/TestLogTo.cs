// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.UnitTests.Chapter17Tests
{
    public class TestLogTo //#A
    {
        private readonly ITestOutputHelper _output; //#B

        public TestLogTo(ITestOutputHelper output)//#C
        {                                         //#C
            _output = output;                     //#C
        }                                         //#C

        [Fact]
        public void TestLogToDemoToConsole()  //#D
        {
            //SETUP
            var connectionString = 
                this.GetUniqueDatabaseConnectionString(); //#E
            var builder =                                    //#F
                new DbContextOptionsBuilder<BookDbContext>() //#F
                .UseSqlServer(connectionString)              //#F
                .EnableSensitiveDataLogging()  //#G
                .LogTo(_output.WriteLine);     //#H

            using var context = new BookDbContext(builder.Options);
            context.Database.EnsureClean();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            var books = context.Books
                .Where(x => x.PublishedOn < new DateTime(2020,1,1))
                .ToList();

            //VERIFY
        }
        /*******************************************************************
        #A This is the class holding my unit tests of LogTo
        #B This is an xUnit interface which allows output to the unit test runner
        #C xUnit will inject the ITestOutputHelper via the class's constructor
        #D This method contains a test of LogTo
        #E This provides a database connection where the database name is unique to this class
        #F This sets up the option builder to a SQL Server database
        #G It is good to turn on EnableSensitiveDataLogging in your unit tests
        #H We add the simplest form of the LogTo method, which calls an Action<string> method
         ******************************************************************/

        [Fact]
        public void TestLogToDemoToList()
        {
            //SETUP
            var connectionString = this.GetUniqueDatabaseConnectionString();

            var logs = new List<string>();   //#A
            var builder =  //#B
                new DbContextOptionsBuilder<BookDbContext>() //#B
                .UseSqlServer(connectionString) //#C
                .EnableSensitiveDataLogging() //#D
                .LogTo(log => logs.Add(log), //#E
                    LogLevel.Information); //#F
            using var context = new BookDbContext(builder.Options); //#G
            //... your query goes here

            /*******************************************************
            #A This will hold all the logs that EF Core outputs
            #B The  DbContextOptionsBuilder<T> is the way to build the options
            #C This says you are using a SQL Server database and takes in a connection string
            #D By default exceptions don't contain sensitive data. This includes sensitive data
            #E The log string is captured and added to the log
            #F This sets the log level - Information level contains the executed SQL
            #G This creates the application's DbContext, in this case the context holding the books data
             ***********************************************************/

            context.Database.EnsureClean();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            logs.Clear();
            var books = context.Books
                .Where(x => x.PublishedOn < new DateTime(2020, 1, 1))
                .ToList();

            //VERIFY
            foreach (var log in logs)
            {
               _output.WriteLine(log); 
            }
        }

        [Fact]
        public void TestLogToDemoList()
        {
            //SETUP
            var connectionString = this.GetUniqueDatabaseConnectionString();
            var showLogs = false;
            var builder = new DbContextOptionsBuilder<BookDbContext>()
                .UseSqlServer(connectionString)
                .EnableSensitiveDataLogging()
                .LogTo( log =>
                {
                    if (showLogs)
                        _output.WriteLine(log);
                }, LogLevel.Information);

            using var context = new BookDbContext(builder.Options);
            context.Database.EnsureClean();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            showLogs = true;
            var books = context.Books
                .Where(x => x.PublishedOn < new DateTime(2020, 1, 1))
                .ToList();

            //VERIFY
        }

        [Fact]
        public void TestLogToCreateDatabaseToConsole()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>(builder => builder.LogTo(_output.WriteLine, LogLevel.Information));
            using var context = new BookDbContext(options);

            //ATTEMPT
            context.Database.EnsureCreated();


            //VERIFY
        }





    }
}