// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter08Listings.EFCode;
using Test.Chapter08Listings.SplitOwnClasses;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch08_TableSplitting
    {
        public Ch08_TableSplitting(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;

        //-----------------------------------------------
        //private helper method
        private static void AddBookSummaryWithDetails(SplitOwnDbContext context)
        {
            var entity = new BookSummary
            {
                Title = "Title",
                AuthorsString = "AuthorA, AuthorB",
                Details = new BookDetail
                {
                    Description = "Description",
                    Price = 10
                }
            };
            context.Add(entity);
            context.SaveChanges();
        }
        //---------------------------------------------------

        [Fact]
        public void TestCreateBookSummaryWithDetailOk()
        {
            //SETUP
            var showLog = false;
            var options = this.CreateUniqueClassOptionsWithLogging<SplitOwnDbContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.ToString());
            });
            using (var context = new SplitOwnDbContext(options))
            {
                context.Database.EnsureClean();

                //ATTEMPT
                showLog = true;
                AddBookSummaryWithDetails(context);
                showLog = false;

                //VERIFY
                context.BookSummaries.Count().ShouldEqual(1);
                context.BookSummaries.Single().BookSummaryId.ShouldNotEqual(0);
            }
        }

        [Fact]
        public void TestCreateBookSummaryWithoutDetailsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SplitOwnDbContext>();
            using var context = new SplitOwnDbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var entity = new BookSummary
            {
                Title = "Title",
                AuthorsString = "AuthorA, AuthorB"
            };
            context.Add(entity);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //VERIFY
            var readEntity = context.BookSummaries.Single();
            readEntity.Details.ShouldBeNull();
        }

        [Fact]
        public void TestReadBookSummaryAndDetailOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SplitOwnDbContext>();
            using var context = new SplitOwnDbContext(options);
            context.Database.EnsureCreated();
            AddBookSummaryWithDetails(context);

            context.ChangeTracker.Clear();

            //ATTEMPT
            var entity = context.BookSummaries.Include(p => p.Details).First();

            //VERIFY
            entity.Details.ShouldNotBeNull();
        }

        [Fact]
        public void TestReadBookSummaryOnlyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SplitOwnDbContext>();
            using var context = new SplitOwnDbContext(options);
            context.Database.EnsureCreated();
            AddBookSummaryWithDetails(context);

            context.ChangeTracker.Clear();

            //ATTEMPT
            var entity = context.BookSummaries.First();

            //VERIFY
            entity.Details.ShouldBeNull();
        }

        [Fact]
        public void TestUpdateBookSummaryOnlyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SplitOwnDbContext>();
            using var context = new SplitOwnDbContext(options);
            context.Database.EnsureCreated();
            AddBookSummaryWithDetails(context);

            context.ChangeTracker.Clear();

            //ATTEMPT
            var entity = context.BookSummaries.First();
            entity.Title = "New Title";
            context.SaveChanges();

            //VERIFY
            context.BookSummaries.First().Title.ShouldEqual("New Title");
        }

        [Fact]
        public void TestUpdateDetailsOnlyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SplitOwnDbContext>();
            using var context = new SplitOwnDbContext(options);
            context.Database.EnsureCreated();
            AddBookSummaryWithDetails(context);

            context.ChangeTracker.Clear();

            //ATTEMPT
            var entity = context.Set<BookDetail>().First();
            entity.Price = 1000;
            context.SaveChanges();

            //VERIFY
            context.Set<BookDetail>().First().Price.ShouldEqual(1000);
        }
    }
}