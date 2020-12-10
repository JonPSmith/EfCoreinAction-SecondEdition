// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.Chapter17Tests
{
    public class TestOptionsWithLogTo
    {
        private readonly ITestOutputHelper _output;

        public TestOptionsWithLogTo(ITestOutputHelper output) 
        {
            _output = output;
        }

        [Fact]
        public void TestEfCoreLoggingExampleOfOutputToConsole()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptionsWithLogTo<BookDbContext>(_output.WriteLine);
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();
                
            //ATTEMPT
            var books = context.Books.ToList(); 

            //VERIFY                                    
        }

        [Fact]
        public void TestEfCoreLoggingCheckSqlOutput()
        {
            //SETUP
            var logs = new List<string>();
            var options = SqliteInMemory.CreateOptionsWithLogTo<BookDbContext>(log => logs.Add(log));
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT 
            var book = context.Books.Count();

            //VERIFY
            logs.Last().ShouldEqual("Executed DbCommand (0ms) [Parameters=[], CommandType='Text', CommandTimeout='30']\r\n" +
                                    "SELECT COUNT(*)\r\nFROM \"Books\" AS \"b\"\r\nWHERE NOT (\"b\".\"SoftDeleted\")");
        }

        [Fact]
        public void TestEfCoreLoggingCheckOnlyShowTheseCategories()
        {
            //SETUP
            var logs = new List<string>();
            var logToOptions = new LogToOptions
            {
                OnlyShowTheseCategories = new[] {DbLoggerCategory.Database.Command.Name}
            };
            var options = SqliteInMemory.CreateOptionsWithLogTo<BookDbContext>(log => logs.Add(log), logToOptions);
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT 
            var books = context.Books.Select(x => x.BookId).ToList();

            //VERIFY
            logs.All(x => x.StartsWith("Executed DbCommand")).ShouldBeTrue();
        }

        [Fact]
        public void TestEfCoreLoggingCheckOnlyShowTheseEvents()
        {
            //SETUP
            var logs = new List<string>();
            var logToOptions = new LogToOptions
            {
                OnlyShowTheseEvents = new[] { CoreEventId.ContextInitialized }
            };
            var options = SqliteInMemory.CreateOptionsWithLogTo<BookDbContext>(log => logs.Add(log), logToOptions);
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT 
            var books = context.Books.Select(x => x.BookId).ToList();

            //VERIFY
            logs.Count.ShouldEqual(1);
            logs.Single().ShouldStartWith("Entity Framework Core 5.0.0 initialized 'BookDbContext' using provider 'Microsoft.EntityFrameworkCore.Sqlite' with options: ");
        }

        [Fact]
        public void TestEfCoreLoggingCheckFilterFunction()
        {
            bool MyFilterFunction(EventId eventId, LogLevel logLevel)
            {
                return eventId.Name == RelationalEventId.CommandExecuted.Name && logLevel == LogLevel.Information;
            }

            //SETUP
            var logs = new List<string>();
            var logToOptions = new LogToOptions
            {
                FilterFunction = MyFilterFunction
            };
            var options = SqliteInMemory.CreateOptionsWithLogTo<BookDbContext>(log => logs.Add(log), logToOptions);
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT 
            var books = context.Books.Select(x => x.BookId).ToList();

            //VERIFY
            logs.All(l => l.StartsWith("Executed DbCommand ")).ShouldBeTrue();
        }

        [Fact]
        public void TestEfCoreLoggingLogToOptionBad()
        {
            //SETUP
            var logs = new List<string>();
            var logToOptions = new LogToOptions
            {
                OnlyShowTheseEvents = new[] { CoreEventId.ContextInitialized },
                OnlyShowTheseCategories = new[] { DbLoggerCategory.Database.Command.Name }
            };

            //ATTEMPT 
            var ex = Assert.Throws<NotSupportedException>(() =>
                SqliteInMemory.CreateOptionsWithLogTo<BookDbContext>(log => logs.Add(log), logToOptions));

            //VERIFY
            ex.Message.ShouldEqual("You can't define OnlyShowTheseCategories and OnlyShowTheseEvents at the same time.");
        }

        [Fact]
        public void TestEfCoreLoggingCheckLoggerOptions()
        {
            //SETUP
            var logs = new List<string>();
            var logToOptions = new LogToOptions
            {
                LoggerOptions = DbContextLoggerOptions.DefaultWithUtcTime
            };
            var options = SqliteInMemory.CreateOptionsWithLogTo<BookDbContext>(log => logs.Add(log), logToOptions);
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT 
            var books = context.Books.Select(x => x.BookId).ToList();

            //VERIFY
            logs.All(x => x.StartsWith("warn:") || x.StartsWith("info:")).ShouldBeTrue();
        }


        [Fact]
        public void TestCreateUniqueClassOptionsWithLogTo()
        {
            //SETUP
            var logs = new List<string>();
            var options = this.CreateUniqueClassOptionsWithLogTo<BookDbContext>(log => logs.Add(log));
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT 
            var book = context.Books.Where(x => x.Reviews.Count() > 1).Select(x => x.BookId).First();

            //VERIFY
            var lines = logs.Last().Split('\n').Select(x => x.Trim()).ToArray();
            lines[1].ShouldEqual("SELECT TOP(1) [b].[BookId]");
            lines[2].ShouldEqual("FROM [Books] AS [b]");
            lines[3].ShouldEqual("WHERE ([b].[SoftDeleted] <> CAST(1 AS bit)) AND ((");
            lines[4].ShouldEqual("SELECT COUNT(*)");
            lines[5].ShouldEqual("FROM [Review] AS [r]");
            lines[6].ShouldEqual("WHERE [b].[BookId] = [r].[BookId]) > 1)");
        }

        [Fact]
        public void TestCreateUniqueMethodOptionsWithLogTo()
        {
            //SETUP
            var logs = new List<string>();
            var options = this.CreateUniqueMethodOptionsWithLogTo<BookDbContext>(log => logs.Add(log));
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT 
            var book = context.Books.Where(x => x.Reviews.Count() > 1).Select(x => x.BookId).First();

            //VERIFY
            var lines = logs.Last().Split('\n').Select(x => x.Trim()).ToArray();
            lines[1].ShouldEqual("SELECT TOP(1) [b].[BookId]");
            lines[2].ShouldEqual("FROM [Books] AS [b]");
            lines[3].ShouldEqual("WHERE ([b].[SoftDeleted] <> CAST(1 AS bit)) AND ((");
            lines[4].ShouldEqual("SELECT COUNT(*)");
            lines[5].ShouldEqual("FROM [Review] AS [r]");
            lines[6].ShouldEqual("WHERE [b].[BookId] = [r].[BookId]) > 1)");
        }
    }
}