// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApp.ServiceLayer.DefaultSql.Books.Dtos;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;
using Test.Chapter13Listings.EfClasses;
using Test.Chapter13Listings.EfCode;
using Test.Chapter13Listings.Repositories;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.Chapter13Tests
{
    public class TestBookRepository
    {
        [Fact]
        public void TestCreateBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<DddContext>();
            using var context = new DddContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            AddBookWithNewAuthor(context);

            //VERIFY
            context.ChangeTracker.Clear();
            var bookWithAuthor = context.Books
                .Include(x => x.AuthorsLink).ThenInclude(x => x.Author)
                .Single();
            bookWithAuthor.Title.ShouldEqual("Test");
            bookWithAuthor.PublishedOn.ShouldEqual(new DateTime(2019, 12, 31));
            bookWithAuthor.AuthorsLink.SingleOrDefault()?.Author.Name.ShouldEqual("Author1");
        }

        [Fact]
        public async Task TestGetBooksViaRepositoryOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<DddContext>();
            using var context = new DddContext(options);
            context.Database.EnsureCreated();
            AddBookWithNewAuthor(context, "Title1");
            AddBookWithNewAuthor(context, "Title2");

            var repository = new BookRepository(context);

            //ATTEMPT
            context.ChangeTracker.Clear();
            var books = repository.GetEntities().ToList();

            //VERIFY
            books.Count.ShouldEqual(2);
            books.Select(b => b.Title).ShouldEqual(new []{ "Title1" , "Title2" });
        }

        [Fact]
        public async Task TestUpdatePublishedOnViaRepositoryOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<DddContext>();
            using var context = new DddContext(options);
            context.Database.EnsureCreated();
            AddBookWithNewAuthor(context);

            var repository = new BookRepository(context);

            //ATTEMPT
            var book = await repository.FindEntityAsync(1);
            book.UpdatePublishedOn(new DateTime(2020,1,1));
            await repository.PersistDataAsync();

            //VERIFY
            context.ChangeTracker.Clear();
            var readBook = context.Books.Single();
            readBook.PublishedOn.ShouldEqual(new DateTime(2020, 1, 1));
        }

        [Fact]
        public async Task TestAddReviewViaRepositoryOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<DddContext>();
            using var context = new DddContext(options);
            context.Database.EnsureCreated();
            AddBookWithNewAuthor(context);

            var repository = new BookRepository(context);

            //ATTEMPT
            var book = repository.LoadBookWithReviews(1);
            book.AddReview(5,"great", "me");
            await repository.PersistDataAsync();

            //VERIFY
            context.ChangeTracker.Clear();
            var readBook = context.Books.Include(b => b.Reviews).Single();
            readBook.Reviews.Count.ShouldEqual(1);
        }

        [Fact]
        public async Task TestExampleAddPromotionOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<DddContext>();
            using var context = new DddContext(options);
            context.Database.EnsureCreated();
            AddBookWithNewAuthor(context);

            var controller = new ExampleAddPromotion(new BookRepository(context));
   
            //ATTEMPT
            var dto = await controller.AddPromotion(1);
            dto.ActualPrice = 99;
            dto.PromotionalText = "$99 today";
            await controller.AddPromotion(dto);

            //VERIFY
            context.ChangeTracker.Clear();
            dto.Title.ShouldEqual("Test");
            dto.OrgPrice.ShouldEqual(123);
            var readBook = context.Books.Single();
            readBook.ActualPrice.ShouldEqual(99);
            readBook.PromotionalText.ShouldEqual("$99 today");
        }

        //------------------------------------------------------------------------
        //helper methods

        private static IStatusGeneric<Book> AddBookWithNewAuthor(DddContext context, string title = "Test")
        {
            var status = Book.CreateBook(title, new DateTime(2019, 12, 31),
                123, new List<Author> {new Author("Author1", "a@gmail.com")});
            status.IsValid.ShouldBeTrue(status.GetAllErrors());
            context.Add(status.Result);
            context.SaveChanges();
            return status;
        }
    }
}