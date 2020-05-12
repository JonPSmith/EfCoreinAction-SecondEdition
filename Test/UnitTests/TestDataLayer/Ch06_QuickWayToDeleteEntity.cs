// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.BookServices.Concrete;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch06_QuickWayToDeleteEntity
    {
        [Fact]
        public void TestDeleteBookNoRelationshipsOk()
        {
            //SETUP
            int bookId;
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var book = new Book { Title = "Test Book" };
                context.Add(book);
                context.SaveChanges();
                bookId = book.BookId;
            }
            //ATTEMPT
            using (var context = new EfCoreContext(options))
            {
                var book = new Book    //#A
                {
                    BookId = bookId    //#B
                };
                context.Remove(book);  //#C
                context.SaveChanges(); //#D
                /*****************************************************
                 #A This creates the entity class that we want to delete - in this case a Book
                 #B This sets the primary key of the entity instance
                 #C The call to Remove tells EF Core you want this entity/row to be deleted
                 #D Then SaveChanges sends the command to the database to delete that row
                 ****************************************************/
            }
            //VERIFY
            using (var context = new EfCoreContext(options))
            {
                context.Books.Count().ShouldEqual(0);
            }
        }

        [Fact]
        public void TestDeleteBookWithDependentEntityCascadeDeleteOk()
        {
            //SETUP
            int bookId;
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var book = new Book
                {
                    Title = "Test Book",
                    Reviews = new List<Review>{new Review()}
                };
                context.Add(book);
                context.SaveChanges();
                bookId = book.BookId;
                context.Books.Count().ShouldEqual(1);
                context.Set<Review>().Count().ShouldEqual(1);
            }
            //ATTEMPT
            using (var context = new EfCoreContext(options))
            {
                var book = new Book { BookId = bookId };
                context.RemoveRange(book);
                context.SaveChanges();
            }
            //VERIFY
            using (var context = new EfCoreContext(options))
            {
                context.Books.Count().ShouldEqual(0);
                context.Set<Review>().Count().ShouldEqual(0);
            }
        }

        [Fact]
        public void TestDeletePriceOfferReadThenRemeoveOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var book = new Book
                {
                    Title = "Test Book",
                    Promotion = new PriceOffer{NewPrice = 1}
                };
                context.Add(book);
                context.SaveChanges();
                context.Books.Count().ShouldEqual(1);
                context.PriceOffers.Count().ShouldEqual(1);
            }
            //ATTEMPT
            using (var context = new EfCoreContext(options))
            {
                var bookWithPromotion = context.Books
                    .Include(x => x.Promotion).Single();
                context.RemoveRange(bookWithPromotion.Promotion);
                context.SaveChanges();
                bookWithPromotion.Promotion.ShouldBeNull();
            }
            //VERIFY
            using (var context = new EfCoreContext(options))
            {
                context.Books.Count().ShouldEqual(1);
                context.PriceOffers.Count().ShouldEqual(0);
            }
        }
    }
}