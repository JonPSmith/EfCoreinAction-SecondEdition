// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

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
        public async Task TestAddReview()
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
    }
}