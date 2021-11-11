// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books.Dtos;
using BookApp.ServiceLayer.DefaultSql.Books.QueryObjects;
using BookApp.ServiceLayer.DefaultSql.Books.Services;
using BookApp.ServiceLayer.DisplayCommon.Books;
using BookApp.ServiceLayer.DisplayCommon.Books.Dtos;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.UnitTests.TestPersistenceSqlBooks
{
    public class TestBetterLinq
    {
        private ITestOutputHelper _output;

        private bool showLogs;
        private readonly DbContextOptions<BookDbContext> _options;
        private SortFilterPageOptions _sfpOptions;

        public TestBetterLinq(ITestOutputHelper output)
        {
            _output = output;
            showLogs = false;
            int count = 1;
            _options = this.CreateUniqueClassOptionsWithLogTo<BookDbContext>(log =>
            {
                if (showLogs)
                {
                    _output.WriteLine($"\nLOG {count++:D2}");
                    _output.WriteLine(log.ToString());
                }
            });

            _sfpOptions = new SortFilterPageOptions
            {
                PageSize = 10,
                PageNum = 0
            };

            using var context = new BookDbContext(_options);
            {
                context.Database.EnsureCreated();
                if (!context.Books.Any())
                {
                    context.SeedDatabaseDummyBooks(100);
                }
            }
        }

        [Fact]
        public async Task TestListBooksServiceOrderNormalOk()
        {
            //SETUP
            using var context = new BookDbContext(_options);

            var service = new ListBooksService(context);
            var sfpOptions = new SortFilterPageOptions();

            //ATTEMPT
            var query = (await service.SortFilterPageAsync(sfpOptions));

            //VERIFY
            _output.WriteLine(query.ToQueryString());
        }

        [Fact]
        public async Task TestListBooksServiceOrderByVotesOk()
        {
            //SETUP
            using var context = new BookDbContext(_options);

            var service = new ListBooksService(context);
            var sfpOptions = new SortFilterPageOptions {OrderByOptions = OrderByOptions.ByVotes};

            //ATTEMPT
            var query = (await service.SortFilterPageAsync(sfpOptions));

            //VERIFY
            _output.WriteLine(query.ToQueryString());
        }


        [Fact]
        public void TestOrderByVotesLinqQuerySyntaxOk()
        {
            //SETUP
            using var context = new BookDbContext(_options);

            //ATTEMPT
            showLogs = true;
            var query = from b in context.Books
                let averageVotes = b.Reviews.Select(y => (double?) y.NumStars).Average()
                orderby averageVotes
                select new {b.BookId, b.Title, averageVotes};

            //VERIFY
            _output.WriteLine(query.ToQueryString());
        }

        [Fact]
        public void ComparePerformanceSeparateOk()
        {
            //SETUP


            //ATTEMPT
            RunManyTests("SortVoteNoRels", SortVotesNoRelationships, 10, 100, 100, 100);
            RunManyTests("SortVoteWithRels", SortVotesWithRelationships, 10,100,100,100);
            RunManyTests("SortVotesSplitQueries", SortVotesWithSplitQueries, 10, 100, 100, 100);            
            RunManyTests("SortVoteRelsAfter", SortVotesRelationshipsAddedAfter, 10, 100, 100, 100);

            //VERIFY

        }


        //--------------------------------------------------------

        private void SortVotesNoRelationships(BookDbContext context)
        {
            var books = context.Books
                .AsNoTracking()
                .Select(p => new BookListDto
                {
                    BookId = p.BookId,
                    Title = p.Title,

                    ReviewsCount = p.Reviews.Count(),
                    ReviewsAverageVotes =
                        p.Reviews.Select(y =>
                            (double?)y.NumStars).Average(),
                }).OrderBy(x => x.ReviewsAverageVotes)
                .ToList();
        }

        private void SortVotesWithRelationships(BookDbContext context)
        {
            var books = context.Books
                .AsNoTracking()
                .Select(p => new BookListDto
            {
                BookId = p.BookId,          
                Title = p.Title,           

                AuthorsOrdered = string.Join(", ",
                    p.AuthorsLink                         
                        .OrderBy(q => q.Order)        
                        .Select(q => q.Author.Name)),
                TagStrings = p.Tags           
                    .Select(x => x.TagId).ToArray(),      
                ReviewsCount = p.Reviews.Count(),
                ReviewsAverageVotes =                   
                    p.Reviews.Select(y =>                  
                        (double?)y.NumStars).Average(),
            }).OrderBy(x => x.ReviewsAverageVotes)
                .ToList();
        }

        private void SortVotesWithSplitQueries(BookDbContext context)
        {
            var books = context.Books
                .AsNoTracking()
                .AsSplitQuery()
                .Include(b => b.AuthorsLink).ThenInclude(x => x.Author)
                .Include(b => b.Reviews)
                .Include(b => b.Tags)
                .OrderBy(x => x.ReviewsAverageVotes)
                .ToList()
                .Select(b => new BookListDto
                {
                    BookId = b.BookId,
                    Title = b.Title,

                    AuthorsOrdered = string.Join(", ",
                        b.AuthorsLink
                            .OrderBy(q => q.Order)
                            .Select(q => q.Author.Name)),
                    TagStrings = b.Tags
                        .Select(x => x.TagId).ToArray(),
                    ReviewsCount = b.Reviews.Count(),
                    ReviewsAverageVotes =
                        b.Reviews.Select(y =>
                            (double?)y.NumStars).Average(),
                });
        }

        private void SortVotesRelationshipsAddedAfter(BookDbContext context)
        {
            var books = context.Books
                .AsNoTracking()
                .Select(p => new BookListDto
            {
                BookId = p.BookId,
                Title = p.Title,

                ReviewsCount = p.Reviews.Count(),
                ReviewsAverageVotes =
                    p.Reviews.Select(y =>
                        (double?)y.NumStars).Average(),
            }).OrderBy(x => x.ReviewsAverageVotes)
                .ToList();

            books.ForEach(b =>
            { 
                b.AuthorsOrdered = string.Join(", ", context.Set<BookAuthor>().AsNoTracking()
                    .Where(x => x.BookId == b.BookId)
                    .OrderBy(q => q.Order)
                    .Select(q => q.Author.Name));
                b.TagStrings = context.Set<BookTag>().AsNoTracking()
                    .Where(x => x.BookId == b.BookId)
                    .Select(x => x.TagId).ToArray();
            });
        }


        private void RunManyTests(string testType, Action<BookDbContext> actionToRun, params int[] numRuns)
        {

            foreach (var numCycles in numRuns)
            {
                RunTest(numCycles, testType, actionToRun);
            }
        }

        private void RunTest(int numCyclesToRun, string testType, Action<BookDbContext> actionToRun)
        {
            _output.WriteLine($"Starting test of '{testType}' with new DbContext ----------------------");
            using var context = new BookDbContext(_options);
            using (new TimeThings(_output, testType, numCyclesToRun))
            {
                for (int i = 0; i < numCyclesToRun; i++)
                {
                    actionToRun(context);
                }
            }
        }

    }
}