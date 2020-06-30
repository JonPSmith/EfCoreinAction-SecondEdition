// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.SoftDeleteServices.Concrete;
using Test.Chapter11Listings;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch11_SoftDeleteService
    {
        [Fact]
        public void TestAddBookWithReviewOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SoftDelDbContext>();
            using (var context = new SoftDelDbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                AddBookWithReviewToDb(context);
            }
            using (var context = new SoftDelDbContext(options))
            {
                //VERIFY
                var book = context.Books.Include(x => x.Reviews).Single();
                book.Title.ShouldEqual("test");
                book.Reviews.ShouldNotBeNull();
                book.Reviews.Single().NumStars.ShouldEqual(1);
            }
        }

        [Fact]
        public void TestSoftDeleteServiceSetSoftDeleteOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SoftDelDbContext>();
            using (var context = new SoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var book = AddBookWithReviewToDb(context);

                var service = new SoftDeleteService(context);

                //ATTEMPT
                service.SetSoftDelete(book);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
            using (var context = new SoftDelDbContext(options))
            {
                context.Books.Count().ShouldEqual(0);
                context.Books.IgnoreQueryFilters().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestSoftDeleteServiceSetSoftDeleteViaKeysOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SoftDelDbContext>();
            using (var context = new SoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var book = AddBookWithReviewToDb(context);

                var service = new SoftDeleteService(context);

                //ATTEMPT
                var wasFound = service.SetSoftDeleteViaKeys<BookSoftDel>(book.BookSoftDelId);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                wasFound.ShouldBeTrue();
            }
            using (var context = new SoftDelDbContext(options))
            {
                context.Books.Count().ShouldEqual(0);
                context.Books.IgnoreQueryFilters().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestSoftDeleteServiceResetSoftDeleteOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SoftDelDbContext>();
            using (var context = new SoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var book = AddBookWithReviewToDb(context);

                var service = new SoftDeleteService(context);
                service.SetSoftDelete(book);
                service.IsValid.ShouldBeTrue(service.GetAllErrors());

                //ATTEMPT
                service.ResetSoftDelete(book);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
            using (var context = new SoftDelDbContext(options))
            {
                context.Books.Count().ShouldEqual(1);
                context.Books.IgnoreQueryFilters().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestSoftDeleteServiceResetSoftDeleteViaKeysOk()
        {
            //SETUP
            int bookId;
            var options = SqliteInMemory.CreateOptions<SoftDelDbContext>();
            using (var context = new SoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                bookId = AddBookWithReviewToDb(context).BookSoftDelId;

                var service = new SoftDeleteService(context);
                service.SetSoftDeleteViaKeys<BookSoftDel>(bookId);
                service.IsValid.ShouldBeTrue(service.GetAllErrors());

                //ATTEMPT
                service.ResetSoftDeleteViaKeys<BookSoftDel>(bookId);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
            using (var context = new SoftDelDbContext(options))
            {
                context.Books.Count().ShouldEqual(1);
                context.Books.IgnoreQueryFilters().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestSoftDeleteServiceGetSoftDeletedEntriesOk()
        {
            //SETUP
            int bookId;
            var options = SqliteInMemory.CreateOptions<SoftDelDbContext>();
            using (var context = new SoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var book1 = AddBookWithReviewToDb(context, "test1");
                var book2 = AddBookWithReviewToDb(context, "test2");

                var service = new SoftDeleteService(context);
                service.SetSoftDelete(book1);
                service.IsValid.ShouldBeTrue(service.GetAllErrors());

            }
            using (var context = new SoftDelDbContext(options))
            {
                var service = new SoftDeleteService(context);

                //ATTEMPT
                var softDelBooks = service.GetSoftDeletedEntries<BookSoftDel>().ToList();

                //VERIFY
                softDelBooks.Count.ShouldEqual(1);
                softDelBooks.Single().Title.ShouldEqual("test1");
                context.Books.Count().ShouldEqual(1);
                context.Books.IgnoreQueryFilters().Count().ShouldEqual(2);
            }
        }


        private static BookSoftDel AddBookWithReviewToDb(SoftDelDbContext context, string title = "test")
        {
            var book = new BookSoftDel
                {Title = title, Reviews = new List<ReviewSoftDel> {new ReviewSoftDel {NumStars = 1}}};
            context.Add(book);
            context.SaveChanges();
            return book;
        }
    }
}