// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.UnitTests.Chapter14
{
    public class TestSyncAsyncPerformance
    {
        private readonly ITestOutputHelper _output;

        private readonly DbContextOptions<BookDbContext> _options;
        private readonly int _firstBookId;

        public TestSyncAsyncPerformance(ITestOutputHelper output)
        {
            _output = output;
            _options = this.CreateUniqueClassOptions<BookDbContext>();

            using var context = new BookDbContext(_options);
            {
                context.Database.EnsureCreated();
                if (context.Books.Count() > 1000)
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }
                if (!context.Books.Any())
                {
                    context.SeedDatabaseDummyBooks(1000);
                }
                _firstBookId = context.Books.First().BookId;
            }
        }

        [Fact]
        public async Task PerformanceTests()
        {
            //SETUP
            using var context = new BookDbContext(_options);

            //ATTEMPT
            RunManyTests("Just Book", context.Books, 100,100,100,100);
            await RunManyTestsAsync("Just Book", context.Books, 100,100,100,100);
            _output.WriteLine("---------------------------------------------");
            RunManyTests("Book with Includes", EagerLoadingBook(context), 100,100,100,100);
            await RunManyTestsAsync("Book with Includes", EagerLoadingBook(context), 100,100,100,100);
            _output.WriteLine("---------------------------------------------");
            RunManyTests("Book with Includes - split", EagerLoadingBookSplit(context), 100,100,100,100);
            await RunManyTestsAsync("Book with Includes - split", EagerLoadingBookSplit(context), 100,100,100,100);
            _output.WriteLine("---------------------------------------------");
            RunManyTests("BookInclude+filterSort", EagerBookSortPriceFilterReviews(context), 100,100,100,100);
            await RunManyTestsAsync("BookInclude+filterSort", EagerBookSortPriceFilterReviews(context), 100,100,100,100);


            //VERIFY
        }


        //-------------------------------------------------------------------------

        private IQueryable<Book> EagerLoadingBook(BookDbContext context)
        {
            return context.Books
                .AsNoTracking()
                .Include(x => x.AuthorsLink)
                .ThenInclude(x => x.Author)
                .Include(x => x.Reviews)
                .Include(x => x.Tags);
        }
        private IQueryable<Book> EagerLoadingBookSplit(BookDbContext context)
        {
            return context.Books
                .AsSplitQuery()
                .AsNoTracking()
                .Include(x => x.AuthorsLink)
                .ThenInclude(x => x.Author)
                .Include(x => x.Reviews)
                .Include(x => x.Tags);
        }


        private IQueryable<Book> EagerBookSortPriceFilterReviews(BookDbContext context)
        {
            return context.Books
                .AsNoTracking()
                .Include(x => x.AuthorsLink)
                .ThenInclude(x => x.Author)
                .Include(x => x.Reviews)
                .Include(x => x.Tags)
                .Where(b => b.Reviews.ToList().Count > 3)
                .OrderBy(b => b.ActualPrice);
        }

        private void RunManyTests(string testType, IQueryable<Book> queryToRun, params int[] numRuns)
        {
            foreach (var numCycles in numRuns)
            {
                using (new TimeThings(_output, "Sync: " + testType, numCycles))
                {
                    for (int i = 0; i < numCycles; i++)
                    {
                        queryToRun.Single(b => b.BookId == i + _firstBookId);
                    }
                }
            }
        }

        private async Task RunManyTestsAsync(string testType, IQueryable<Book> queryToRun, params int[] numRuns)
        {
            foreach (var numCycles in numRuns)
            {
                using (new TimeThings(_output, "Async: " + testType, numCycles))
                {
                    for (int i = 0; i < numCycles; i++)
                    {
                        await queryToRun.SingleAsync( b => b.BookId == i + _firstBookId);
                    }
                }
            }
        }

    }
}