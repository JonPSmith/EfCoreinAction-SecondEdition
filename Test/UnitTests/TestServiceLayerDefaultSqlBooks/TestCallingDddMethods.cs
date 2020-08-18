// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books.Dtos;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestServiceLayerDefaultSqlBooks
{
    public class TestCallingDddMethods
    {
        [Fact]
        public async Task TestAddReviewDto()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            var books = context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            var utData = context.SetupSingleDtoAndEntities<AddReviewDto>();
            var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

            //ATTEMPT
            var dto = new AddReviewDto
                {BookId = books[0].BookId, NumStars = 5, Comment = "Great Book", VoterName = "Test"};
            await service.UpdateAndSaveAsync(dto);

            //VERIFY
            var book = context.Books.Include(x => x.Reviews).Single(x => x.BookId == books[0].BookId);
            book.Reviews.Count.ShouldEqual(1);
            book.Reviews.Single().NumStars.ShouldEqual(5);
            book.Reviews.Single().Comment.ShouldEqual("Great Book");
            book.Reviews.Single().VoterName.ShouldEqual("Test");
        }

        [Fact]
        public async Task TestRemoveReviewDto()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            var books = context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            var utData = context.SetupSingleDtoAndEntities<RemoveReviewDto>();
            var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

            //ATTEMPT
            var dto = new RemoveReviewDto { BookId = books[3].BookId, ReviewId = books[3].Reviews.Last().ReviewId };
            await service.UpdateAndSaveAsync(dto);

            //VERIFY
            var book = context.Books.Include(x => x.Reviews).Single(x => x.BookId == books[3].BookId);
            book.Reviews.Count.ShouldEqual(1);
        }

        [Fact]
        public async Task TestChangePubDateDto()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            var books = context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            var utData = context.SetupSingleDtoAndEntities<ChangePubDateDto>();
            var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

            //ATTEMPT
            var dto = new ChangePubDateDto { BookId = books[3].BookId, PublishedOn = new DateTime(2020,1,1)};
            await service.UpdateAndSaveAsync(dto);

            //VERIFY
            var book = context.Books.Single(x => x.BookId == books[3].BookId);
            book.PublishedOn.ShouldEqual(new DateTime(2020, 1, 1));
        }

        [Fact]
        public async Task TestRemovePromotionDto()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            var books = context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            var utData = context.SetupSingleDtoAndEntities<RemovePromotionDto>();
            var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

            //ATTEMPT
            var dto = new RemovePromotionDto { BookId = books[3].BookId };
            await service.UpdateAndSaveAsync(dto, "RemovePromotion");

            //VERIFY
            var book = context.Books.Single(x => x.BookId == books[3].BookId);
            book.ActualPrice.ShouldEqual(book.OrgPrice);
            book.PromotionalText.ShouldBeNull();
        }

        [Fact]
        public async Task TestAddPromotionDto()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            var books = context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            var utData = context.SetupSingleDtoAndEntities<AddPromotionDto>();
            var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

            //ATTEMPT
            var dto = new AddPromotionDto { BookId = books[1].BookId, ActualPrice = 1.23m, PromotionalText = "Save!"};
            await service.UpdateAndSaveAsync(dto);

            //VERIFY
            var book = context.Books.Single(x => x.BookId == books[1].BookId);
            book.ActualPrice.ShouldEqual(1.23m);
            book.OrgPrice.ShouldNotEqual(book.ActualPrice);
            book.PromotionalText.ShouldEqual("Save!");
        }

        [Fact]
        public async Task TestCreateBookDto()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();

            var utData = context.SetupSingleDtoAndEntities<CreateBookDto>();
            var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

            //ATTEMPT
            var dto = new CreateBookDto
            {
                Title = "New book", ImageUrl = "link", Price = 123, PublishedOn = new DateTime(2020, 1, 1), 
                Publisher = "Test", Authors = new []{new Author("Author1", null)}
            };
            await service.CreateAndSaveAsync(dto);

            //VERIFY
            var book = context.Books.Single();
            book.ToString().ShouldEqual("New book: by Author1. Price 123, 0 reviews. Published 01/01/2020");
        }
    }
}