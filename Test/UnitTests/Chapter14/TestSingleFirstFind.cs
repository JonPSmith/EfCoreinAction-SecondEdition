// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using BookApp.Domain.Books;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.UnitTests.Chapter14
{
    public class TestSingleFirstFind
    {
        private readonly ITestOutputHelper _output;

        private readonly DbContextOptions<BookDbContext> _options;
        private readonly int _firstBookId;

        public TestSingleFirstFind(ITestOutputHelper output)
        {
            _output = output;
            _options = this.CreateUniqueClassOptions<BookDbContext>();

            using var context = new BookDbContext(_options);
            {
                context.Database.EnsureCreated();
                if (!context.Books.Any())
                {
                    context.SeedDatabaseDummyBooks(1000);
                }
                _firstBookId = context.Books.First().BookId;
            }
        }

        [Fact]
        public void PerformanceTests()
        {
            //SETUP

            //ATTEMPT
            RunManyTests("First", LoadWithFirst, 1000, 1000, 1000);
            RunManyTests("First", LoadWithFirst, 1000, 1000, 1000);

            RunManyTests("Single", LoadWithSingle, 1000, 1000, 1000);
            RunManyTests("Single", LoadWithSingle, 1000, 1000, 1000);

            RunManyTests("SingleOrDefault", LoadWithSingleOrDefault, 1000, 1000, 1000);
            RunManyTests("SingleOrDefault", LoadWithSingleOrDefault, 1000, 1000, 1000);

            RunManyTests("Find", LoadWithFind, 1000);
            RunManyTests("Find", LoadWithFind, 1000);
            RunManyTests("Find", LoadWithFind, 1000);
            RunManyTests("Find", LoadWithFind, 1000);
            RunManyTests("Find", LoadWithFind, 1000);

            using (var context = new BookDbContext(_options))
            {
                RunTest(context, 1000, "First Already Loaded", LoadWithFind);
                RunTest(context, 1000, "First Already Loaded", LoadWithFind);
                RunTest(context, 1000, "First Already Loaded", LoadWithFind);
                RunTest(context, 1000, "First Already Loaded", LoadWithFind);
            }
            //VERIFY
        }


        //-------------------------------------------------------------------------

        private void RunManyTests(string testType, Action<BookDbContext, int> actionToRun, params int[] numRuns)
        {
            _output.WriteLine($"Starting test of '{testType}' with new DbContext ----------------------");
            using var context = new BookDbContext(_options);
            foreach (var numCycles in numRuns)
            {
                RunTest(context, numCycles, testType, actionToRun);
            }
        }

        private void RunTest(BookDbContext context, int numCyclesToRun, string testType, Action<BookDbContext, int> actionToRun)
        {
            using (new TimeThings(_output, testType, numCyclesToRun))
            {
                for (int i = 0; i < numCyclesToRun; i++)
                {
                    actionToRun(context, i + _firstBookId);
                }
            }
        }

        private void LoadWithFirst(BookDbContext context, int id)
        {
            var book = context.Books.First(x => x.BookId == id);
        }

        private void LoadWithSingle(BookDbContext context, int id)
        {
            var book = context.Books.Single(x => x.BookId == id);
        }

        private void LoadWithSingleOrDefault(BookDbContext context, int id)
        {
            var book = context.Books.SingleOrDefault(x => x.BookId == id);
        }

        private void LoadWithFind(BookDbContext context, int id)
        {
            var book = context.Find<Book>(id);
        }
    }
}