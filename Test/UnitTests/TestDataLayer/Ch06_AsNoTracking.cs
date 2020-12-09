// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using MyFirstEfCoreApp;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch06_AsNoTracking
    {
        private readonly ITestOutputHelper _output;

        public Ch06_AsNoTracking(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestDatabaseNormalQueryOk()
        {
            //SETUP
            using (var context = new AppDbContext())
            {
                Commands.WipeCreateSeed(true);

                //ATTEMPT
                var books = context.Books.Include(x => x.Author).ToList();

                //VERIFY
                books.Count.ShouldEqual(4);
                books.Select(x => x.Author).Distinct().Count().ShouldEqual(3);
                books.Select(x => x.Author.Name).ShouldEqual(new []{ "Martin Fowler", "Martin Fowler", "Eric Evans", "Future Person" });
            }
        }

        [Fact]
        public void TestAsNoTrackingQueryOk()
        {
            //SETUP
            using (var context = new AppDbContext())
            {
                Commands.WipeCreateSeed(true);

                //ATTEMPT
                var books = context.Books.Include(x => x.Author)
                    .AsNoTracking().ToList();

                //VERIFY
                books.Count.ShouldEqual(4);
                books.Select(x => x.Author).Distinct().Count().ShouldEqual(4);
                books.Select(x => x.Author.Name).ShouldEqual(new[] { "Martin Fowler", "Martin Fowler", "Eric Evans", "Future Person" });
            }
        }


        [Fact]
        public void TestAsNoTrackingWithIdentityResolutionOk()
        {
            //SETUP
            using (var context = new AppDbContext())
            {
                Commands.WipeCreateSeed(true);

                //ATTEMPT
                var books = context.Books.Include(x => x.Author)
                    .AsNoTrackingWithIdentityResolution().ToList();

                //VERIFY
                books.Count.ShouldEqual(4);
                books.Select(x => x.Author).Distinct().Count().ShouldEqual(3);
                books.Select(x => x.Author.Name).ShouldEqual(new[] { "Martin Fowler", "Martin Fowler", "Eric Evans", "Future Person" });
            }
        }

        [Fact]
        public void TestQueryPerformanceOk()
        {
            //SETUP
            int numBooks = 100;
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            options.TurnOffDispose();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(numBooks);
            } 
            //ATTEMPT
            using (var context = new EfCoreContext(options))
            using (new TimeThings(_output, "Normal"))
            {
                var books = context.Books
                    .Include(x => x.Reviews)
                    .Include(x => x.AuthorsLink).ThenInclude(x => x.Author)
                    .ToList();
            }
            using (var context = new EfCoreContext(options))
            using (new TimeThings(_output, "AsNoTracking"))
            {
                var books = context.Books
                    .AsNoTracking()
                    .Include(x => x.Reviews)
                    .Include(x => x.AuthorsLink).ThenInclude(x => x.Author)
                    .ToList();
            }
            using (var context = new EfCoreContext(options))
            using (new TimeThings(_output, "AsNoTrackingWithIdentityResolution"))
            {
                var books = context.Books
                    .AsNoTrackingWithIdentityResolution()
                    .Include(x => x.Reviews)
                    .Include(x => x.AuthorsLink).ThenInclude(x => x.Author)
                    .ToList();
            }

            using (var context = new EfCoreContext(options))
            using (new TimeThings(_output, "Normal2"))
            {
                var books = context.Books
                    .Include(x => x.Reviews)
                    .Include(x => x.AuthorsLink).ThenInclude(x => x.Author)
                    .ToList();
            }

            using (var context = new EfCoreContext(options))
            using (new TimeThings(_output, "AsNoTracking2"))
            {
                var books = context.Books
                    .AsNoTracking()
                    .Include(x => x.Reviews)
                    .Include(x => x.AuthorsLink).ThenInclude(x => x.Author)
                    .ToList();
            }

            using (var context = new EfCoreContext(options))
            using (new TimeThings(_output, "AsNoTrackingWithIdentityResolution2"))
            {
                var books = context.Books
                    .AsNoTrackingWithIdentityResolution()
                    .Include(x => x.Reviews)
                    .Include(x => x.AuthorsLink).ThenInclude(x => x.Author)
                    .ToList();
            }
            options.ManualDispose();
        }
    }
}