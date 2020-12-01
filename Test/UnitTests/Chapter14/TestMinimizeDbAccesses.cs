// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.UnitTests.Chapter14
{
    public class TestMinimizeDbAccesses
    {
        private readonly ITestOutputHelper _output;

        private bool showLogs;
        private readonly DbContextOptions<BookDbContext> _options;
        private readonly int _firstBookId;

        public TestMinimizeDbAccesses(ITestOutputHelper output)
        {
            _output = output;
            showLogs = false;
            int count = 1;
            _options = this.CreateUniqueClassOptionsWithLogging<BookDbContext>(log =>
            {
                if (showLogs)
                {
                    _output.WriteLine($"\nLOG {count++:D2}");
                    _output.WriteLine(log.ToString());
                }
            });

            using var context = new BookDbContext(_options);
            {
                context.Database.EnsureCreated();
                if (!context.Books.Any())
                {
                    context.SeedDatabaseDummyBooks(110);
                }
                _firstBookId = context.Books.First().BookId;
            }
        }

        [Fact]
        public void CheckIncludeAndSelect()
        {
            //SETUP
            using var context = new BookDbContext(_options);

            //ATTEMPT
            showLogs = true;

            var bookInclude = context.Books.Include(b => b.Reviews).First();
            _output.WriteLine("Select --------------------------------------");
            var bookSelect = context.Books.Select(b => new
            {
                b.Title,
                Reviews = b.Reviews.ToList()
            }).First();

            //VERIFY
        }


        [Fact]
        public void SelectPerformance()
        {
            //SETUP

            //ATTEMPT
            RunManyTests("SelectLoad:", SelectLoad, 1, 10, 100, 10, 100, 100);
            showLogs = true;
            RunTest(1, "First access, SelectLoad:", SelectLoad);
            showLogs = false;

            //VERIFY
        }

        [Fact]
        public void EagerPerformance()
        {
            //SETUP

            //ATTEMPT
            RunManyTests("EagerLoading:", EagerLoading, 1, 10, 100, 10, 100, 100);
            showLogs = true;
            RunTest(1, "EagerLoad:", EagerLoading);
            showLogs = false;

            //VERIFY
        }

        [Fact]
        public void EagerSplitPerformance()
        {
            //SETUP

            //ATTEMPT
            RunManyTests("EagerSplitLoad:", EagerLoadingSplit, 1, 10,100,10,100,100);
            showLogs = true;
            RunTest(1, "EagerSplitLoad:", EagerLoadingSplit);
            showLogs = false;

            //VERIFY
        }

        [Fact]
        public void ExplicitPerformance()
        {
            //SETUP

            //ATTEMPT
            RunManyTests("ExplicitLoading:", ExplicitLoading, 1, 10, 100, 10, 100, 100);
            showLogs = true;
            RunTest(1, "First access, ExplicitLoading:", ExplicitLoading);
            showLogs = false;

            //VERIFY
        }



        //--------------------------------------------------------

        private void RunManyTests(string testType, Action<BookDbContext, int> actionToRun, params int[] numRuns)
        {
           
            foreach (var numCycles in numRuns)
            {
                RunTest(numCycles, testType, actionToRun);
            }
        }

        private void RunTest(int numCyclesToRun, string testType, Action<BookDbContext, int> actionToRun)
        {
            _output.WriteLine($"Starting test of '{testType}' with new DbContext ----------------------");
            using var context = new BookDbContext(_options);
            using (new TimeThings(_output, testType, numCyclesToRun))
            {
                for (int i = 0 ; i < numCyclesToRun; i++)
                {
                    actionToRun(context, i + _firstBookId);
                }
            }
        }

        private void EagerLoading(BookDbContext context, int id)
        {
            var book = context.Books
                .Include(x => x.AuthorsLink)
                .ThenInclude(x => x.Author)
                .Include(x => x.Reviews)
                .Include(x => x.Tags)
                .Single(x => x.BookId == id);
        }

        private void EagerLoadingSplit(BookDbContext context, int id)
        {
            var book = context.Books
                .AsSplitQuery()
                .Include(x => x.AuthorsLink)
                .ThenInclude(x => x.Author)
                .Include(x => x.Reviews)
                .Include(x => x.Tags)
                .Single(x => x.BookId == id);
        }

        private void ExplicitLoading(BookDbContext context, int id)
        {
            var book = context.Books.Single(x => x.BookId == id);
            context.Entry(book).Collection(c => c.AuthorsLink).Load();
            foreach (var authorLink in book.AuthorsLink)
            {                                        
                context.Entry(authorLink)            
                    .Reference(r => r.Author).Load();
            }
            context.Entry(book).Collection(c => c.Reviews).Load();
            context.Entry(book).Collection(c => c.Tags).Load();
        }

        private void SelectLoad(BookDbContext context, int id)
        {
            var book = context.Books
                .Select(p => new
                {
                    p.BookId,
                    p.Publisher,
                    p.PublishedOn,
                    p.EstimatedDate,
                    p.OrgPrice,
                    p.ActualPrice,
                    p.PromotionalText,
                    p.ImageUrl,
                    p.ManningBookUrl,
                    p.LastUpdatedUtc,
                    ReviewsCount = p.Reviews.Count(),
                    ReviewsVotes = p.Reviews.Select(x => x.NumStars).ToList(),
                    Authors = p.AuthorsLink.OrderBy(x => x.Order).Select(x => x.Author).ToList(),
                    Tags = p.Tags.ToList()
                })
                .Single(x => x.BookId == id);
        }
    }
}