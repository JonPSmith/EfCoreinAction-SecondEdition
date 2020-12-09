// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.Chapter06Listings;
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
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            var bookSetup = new Book { Title = "Test Book" };
            context.Add(bookSetup);
            context.SaveChanges();
            bookId = bookSetup.BookId;
            

            context.ChangeTracker.Clear();

            //ATTEMPT
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

            context.ChangeTracker.Clear();

            //VERIFY
            context.Books.Count().ShouldEqual(0);
        }

        [Fact]
        public void TestDeleteBookWithDependentEntityCascadeDeleteOk()
        {
            //SETUP
            int bookId;
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            var bookSetup = new Book
            {
                Title = "Test Book",
                Reviews = new List<Review>{new Review()}
            };
            context.Add(bookSetup);
            context.SaveChanges();
            bookId = bookSetup.BookId;
            context.Books.Count().ShouldEqual(1);
            context.Set<Review>().Count().ShouldEqual(1);

            context.ChangeTracker.Clear();

            //ATTEMPT
            var book = new Book { BookId = bookId };
            context.Remove(book);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //VERIFY

            context.ChangeTracker.Clear();

            context.Books.Count().ShouldEqual(0);
            context.Set<Review>().Count().ShouldEqual(0);
        }

        [Fact]
        public void TestDeletePriceOfferQuicklyOk()
        {
            //SETUP
            int priceOfferId;
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            var book = new Book
            {
                Title = "Test Book",
                Promotion = new PriceOffer { NewPrice = 1 }
            };
            context.Add(book);
            context.SaveChanges();
            priceOfferId = book.Promotion.PriceOfferId;
            context.Books.Count().ShouldEqual(1);
            context.PriceOffers.Count().ShouldEqual(1);

            context.ChangeTracker.Clear();

            //ATTEMPT
            var pOfferToDelete = new PriceOffer { PriceOfferId = priceOfferId };
            context.Remove(pOfferToDelete);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //VERIFY
            context.Books.Count().ShouldEqual(1);
            context.PriceOffers.Count().ShouldEqual(0);
        }

        [Fact]
        public void TestDeletePriceOfferRemoveNullsNavigatinalLinkOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
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

            context.ChangeTracker.Clear();

            //ATTEMPT
            var bookWithPromotion = context.Books
                .Include(x => x.Promotion).Single();
            context.Remove(bookWithPromotion.Promotion);
            context.SaveChanges();
            bookWithPromotion.Promotion.ShouldBeNull();

            context.ChangeTracker.Clear();

            //VERIFY
            context.Books.Count().ShouldEqual(1);
            context.PriceOffers.Count().ShouldEqual(0);
        }

        [Fact]
        public void TestDeleteBookMissing()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var book = new Book    
            {
                BookId = 123       
            };
            context.Remove(book);  
            var ex = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges()); 

            //VERIFY
            ex.Message.ShouldStartWith("Database operation expected to affect 1 row(s) but actually affected 0 row(s).");
        }

        [Fact]
        public void TestDeleteDependent()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            using var context = new Chapter06Context(options);
            context.Database.EnsureCreated();
            context.Add(new OnePrincipal {Link = new OneDependent()});
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var depToRemove = new OneDependent {Id = 1};
            context.Remove(depToRemove);
            context.SaveChanges();

            //VERIFY
            context.OneDependents.Count().ShouldEqual(0);
            context.OnePrincipals.Count().ShouldEqual(1);
        }

        [Fact]
        public void TestDeletePrincipal()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            using var context = new Chapter06Context(options);
            context.Database.EnsureCreated();
            context.Add(new OnePrincipal { Link = new OneDependent() });
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var depToRemove = new OnePrincipal { Id = 1 };
            context.Remove(depToRemove);
            context.SaveChanges();

            //VERIFY
            context.OneDependents.Count().ShouldEqual(0);
            context.OnePrincipals.Count().ShouldEqual(0);
        }
    }
}