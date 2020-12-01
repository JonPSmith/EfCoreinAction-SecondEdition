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
    public class TestLogTo
    {
        private readonly ITestOutputHelper _output;

        public TestLogTo(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestLogToDemoToConsole()
        {
            //SETUP
            var connectionString = this.GetUniqueDatabaseConnectionString();
            var builder = new DbContextOptionsBuilder<BookDbContext>()
                .UseSqlServer(connectionString)
                .EnableSensitiveDataLogging()
                .LogTo(_output.WriteLine, LogLevel.Information);

            using var context = new BookDbContext(builder.Options);
            context.Database.EnsureCreated();
            if (!context.Books.Any())
                context.SeedDatabaseFourBooks();

            //ATTEMPT
            var books = context.Books
                .Where(x => x.PublishedOn < new DateTime(2020,1,1))
                .ToList();

            //VERIFY
        }

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

            context.Database.EnsureCreated();

            //ATTEMPT
            if (!context.Books.Any())
                context.SeedDatabaseFourBooks();
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
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            showLogs = true;

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