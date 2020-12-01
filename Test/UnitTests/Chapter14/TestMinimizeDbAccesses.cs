// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using BookApp.Domain.Books;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books.Dtos;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
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
            RunTest(1, "SelectLoad:", SelectLoad);
            showLogs = false;

            //VERIFY
        }

        [Fact]
        public void SelectLoadSeparatePerformance()
        {
            //SETUP

            //ATTEMPT
            RunManyTests("SelectLoadSeparate:", SelectLoadSeparate, 1, 10, 100, 10, 100, 100);
            showLogs = true;
            RunTest(1, "SelectLoadSeparate:", SelectLoadSeparate);
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
        public void EagerLoadAuthorSeparatelyPerformance()
        {
            //SETUP

            //ATTEMPT
            RunManyTests("EagerLoadAuthorSeparately:", EagerLoadAuthorSeparately, 1, 10, 100, 10, 100, 100);
            showLogs = true;
            RunTest(1, "EagerLoadAuthorSeparately:", EagerLoadAuthorSeparately);
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

        private void EagerLoadAuthorSeparately(BookDbContext context, int id)
        {
            var bookNoAuthors = context.Books
                .Include(x => x.Reviews)
                .Include(x => x.Tags)
                .Single(x => x.BookId == id);
            var authors = context.Set<BookAuthor>().Include(x => x.Author)
                .Where(x => x.BookId == id).ToList();

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
                .Select(p => new BookListDto
                {
                    BookId = p.BookId,          
                    Title = p.Title,           
                    PublishedOn = p.PublishedOn,     
                    EstimatedDate = p.EstimatedDate,   
                    OrgPrice = p.OrgPrice,        
                    ActualPrice = p.ActualPrice,     
                    PromotionText = p.PromotionalText, 
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
                    ManningBookUrl = p.ManningBookUrl
                })
                .Single(x => x.BookId == id);
        }

        private void SelectLoadSeparate(BookDbContext context, int id)
        {
            var dto = context.Books
                .Select(p => new BookListDto
                {
                    BookId = p.BookId,          
                    Title = p.Title,           
                    PublishedOn = p.PublishedOn,     
                    EstimatedDate = p.EstimatedDate,   
                    OrgPrice = p.OrgPrice,        
                    ActualPrice = p.ActualPrice,     
                    PromotionText = p.PromotionalText, 
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
                    ManningBookUrl = p.ManningBookUrl
                })
                .Single(x => x.BookId == id);
            dto.AuthorsOrdered = string.Join(", ", context.Set<BookAuthor>()
                .Where(x => x.BookId == id)
                .OrderBy(q => q.Order)
                .Select(q => q.Author.Name));

        }
    }
}