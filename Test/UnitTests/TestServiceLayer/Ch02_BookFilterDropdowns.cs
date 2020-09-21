// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using ServiceLayer.BookServices.Concrete;
using ServiceLayer.BookServices.QueryObjects;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestServiceLayer
{
    public class Ch02_BookFilterDropdowns
    {
        [Fact]
        public void DropdownByDate()
        {
            //SETUP
            const int numBooks = 5;
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.Books.AddRange(EfTestData.CreateDummyBooks(numBooks, true));
                context.SaveChanges();
                var service = new BookFilterDropdownService(context);

                //ATTEMPT
                var dropDown = service.GetFilterDropDownValues(BooksFilterBy.ByPublicationYear);

                //VERIFY
                dropDown.Select(x => x.Value).ToArray().ShouldEqual(new[] {"2014", "2013", "2012", "2011", "2010"});
            }
        }

        [Fact]
        public void DropdownByTag()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                var service = new BookFilterDropdownService(context);

                //ATTEMPT
                var dropDown = service.GetFilterDropDownValues(BooksFilterBy.ByTags);

                //VERIFY
                dropDown.Select(x => x.Value).ToArray().ShouldEqual(
                    new[] { "Refactoring", "Editor's Choice", "Architecture", "Quantum Entanglement" });
            }
        }

        ////Test used in https://github.com/dotnet/efcore/issues/21445
        //[Fact]
        //public void TestDistinctClientVsServerEval()
        //{
        //    //SETUP
        //    var options = SqliteInMemory.CreateOptions<EfCoreContext>();
        //    using (var context = new EfCoreContext(options))
        //    {
        //        context.Database.EnsureCreated();
        //        context.Books.AddRange(
        //            new Book { PublishedOn = new DateTime(2001, 1, 1) },
        //            new Book { PublishedOn = new DateTime(2001, 1, 1) },
        //            new Book{PublishedOn = new DateTime(2000,1,1)}
        //            );
        //        context.SaveChanges();


        //        //ATTEMPT
        //        var dates = context.Books
        //            .Select(x => x.PublishedOn.Year)
        //            .Distinct()
        //            .Select(x => new DropdownTuple
        //            {
        //                Value = x.ToString(),
        //                Text = x.ToString()
        //            }).ToList();

        //        //VERIFY
        //        dates.Select(x => x.Value).ToArray().ShouldEqual(new[] { "2001", "2000" });
        //    }
        //}

        [Fact]
        public void DropdownByDateExcludeBooksLaterThanCurrentTimeButInYear()
        {
            //SETUP
            if (DateTime.Today.AddDays(1).Year != DateTime.Today.Year)
                throw new Exception("This unit test will fail if you run it on the last day of the year!");

            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var oldBook = new Book{PublishedOn = new DateTime(2000,1,1)};
                var futureBookThisYear = new Book { PublishedOn = DateTime.UtcNow.Date.AddDays(1) };
                context.Books.AddRange(oldBook, futureBookThisYear);
                context.SaveChanges();
                var service = new BookFilterDropdownService(context);

                //ATTEMPT
                var dropDown = service.GetFilterDropDownValues(BooksFilterBy.ByPublicationYear);

                //VERIFY
                dropDown.Select(x => x.Value).ToArray().ShouldEqual(new[] { BookListDtoFilter.AllBooksNotPublishedString, "2000"});
            }
        }
    }
}