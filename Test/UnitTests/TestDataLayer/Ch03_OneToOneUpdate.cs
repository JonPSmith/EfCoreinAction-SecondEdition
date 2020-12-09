// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch03_OneToOneUpdate
    {
        private readonly ITestOutputHelper _output;

        public Ch03_OneToOneUpdate(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestConnectedUpdateNoExistingRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var book = context.Books                  //#A
                    .Include(p => p.Promotion)            //#B
                    .First(p => p.Promotion == null);

                book.Promotion = new PriceOffer           //#C
                {                                         //#C
                    NewPrice = book.Price / 2,            //#C
                    PromotionalText = "Half price today!" //#C
                };                                        //#C
                context.SaveChanges();                    //#D                  
                /**********************************************************
                #A Finds a book. In this example it doesn’t have an existing promotion, but it would also work if there was an existing promotion
                #B While the include isn't needed because I am loading something without a Promotion it is good practice to include it, as you should load any any relationships if you are going to change a relationship
                #C I add a new PriceOffer to this book
                #D The SaveChanges method calls DetectChanges, which find that the Promotion property has changed, so it adds that entry to the PricerOffers table
                * *******************************************************/

                //VERIFY
                var bookAgain = context.Books 
                    .Include(p => p.Promotion)                    
                    .Single(p => p.BookId == book.BookId);
                bookAgain.Promotion.ShouldNotBeNull();
                bookAgain.Promotion.PromotionalText.ShouldEqual("Half price today!");     
            }
        }

        [Fact]
        public void TestConnectedUpdateIncludeOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var book = context.Books             
                    .Include(p => p.Promotion)       
                    .First(p => p.Promotion != null);

                book.Promotion = new PriceOffer          
                {                                        
                    NewPrice = book.Price / 2,           
                    PromotionalText = "Half price today!"
                };                                       
                context.SaveChanges();                                    

                //VERIFY
                context.PriceOffers.Count().ShouldEqual(1); //there is only one promotion in the four book data
                context.PriceOffers.First().PromotionalText.ShouldEqual("Half price today!");
            }
        }

        [Fact]
        public void TestConnectedUpdateNoIncludeBad()
        {
            //SETUP
            int bookId;
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureClean();
                if (!context.Books.Any())
                    context.SeedDatabaseFourBooks();

                bookId = context.Books.ToList().Last().BookId; //Last has a price promotion
            }
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var book = context.Books
                    .Single(p => p.BookId == bookId);
                book.Promotion = new PriceOffer
                {
                    NewPrice = book.Price / 2,
                    PromotionalText = "Half price today!"
                };
                var ex = Assert.ThrowsAny<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.StartsWith(
                    "Cannot insert duplicate key row in object 'dbo.PriceOffers' with unique index 'IX_PriceOffers_BookId'.")
                    .ShouldBeTrue();
            }
        }

        [Fact]
        public void TestDeleteExistingRelationship()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var book = context.Books
                    .Include(p => p.Promotion)
                    .First(p => p.Promotion != null);

                //ATTEMPT
                book.Promotion = null;
                context.SaveChanges();

                //VERIFY
                var bookAgain = context.Books
                    .Include(p => p.Promotion)
                    .Single(p => p.BookId == book.BookId);
                bookAgain.Promotion.ShouldBeNull();
                context.PriceOffers.Count().ShouldEqual(0);
            }
        }

        //---------------------------------------------------------
        // Create/delete of PriceOffer via its own table

        [Fact]
        public void TestCreatePriceOffer()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var orgPriceOffers = context.PriceOffers.Count();
                var book = context.Books
                    .First(p => p.Promotion == null);     //#A

                //ATTEMPT
                context.Add(new PriceOffer                //#B
                {                                         //#C
                    BookId = book.BookId,                 //#C
                    NewPrice = book.Price / 2,            //#C
                    PromotionalText = "Half price today!" //#C
                });                                       //#C
                context.SaveChanges();                    //#D
                /******************************************************
                #A Here I find the book that I want to add the new PriceOffer to. It must NOT have an existing PriceOffer
                #B I add the new PriceOffer to the PriceOffers table
                #C This defines the PriceOffer. Note that you MUST include the BookId (previously EF Core filled that in)
                #D SaveChanges adds the PriceOffer to the PriceOffers table
                 * *****************************************************/

                //VERIFY
                var bookAgain = context.Books
                    .Include(p => p.Promotion)
                    .Single(p => p.BookId == book.BookId);
                bookAgain.Promotion.ShouldNotBeNull();
                context.PriceOffers.Count().ShouldEqual(orgPriceOffers+1);
            }
        }

        [Fact]
        public void TestDeletePriceOffer()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var book = context.Books
                    .First(p => p.Promotion != null);

                //ATTEMPT
                context.Remove(context.PriceOffers.Find(book.Promotion.PriceOfferId));
                context.SaveChanges();

                //VERIFY
                var bookAgain = context.Books
                    .Include(p => p.Promotion)
                    .Single(p => p.BookId == book.BookId);
                bookAgain.Promotion.ShouldBeNull();
                context.PriceOffers.Count().ShouldEqual(0);
            }
        }

        //-----------------------------------------------------------

        [Fact]
        public void TestDisconnectedUpdateNoExistingRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var bookId = 1;                      //#A
                var newPrice = 12;                   //#A
                var newText = "Half price today!";   //#A

                var book = context.Books             //#B
                    .Include(p => p.Promotion)       //#B
                    .Single(p => p.BookId == bookId);//#B
                book.Promotion = new PriceOffer      //#C
                {                                    //#C
                    NewPrice = newPrice,             //#C
                    PromotionalText = newText        //#C
                };                                   //#C
                context.SaveChanges();               //#D

                /*********************************************************
                #A This simulates receiving the the data passed back after the disconnect. In a browser this would come back from an HTML form 
                #B This code loads the book that the new promotion should be applied to
                #C This forms the PriceOffer to be added to the Book. Note that I don't need to set the BookId - EF Core does that
                #D The SaveChanges method calls DetectChanges, which find that the Promotion property has changed, so it adds that entry to the PricerOffers table
                 * *******************************************************/
                //VERIFY
                var bookAgain = context.Books
                    .Include(p => p.Promotion)
                    .Single(p => p.BookId == book.BookId);
                bookAgain.Promotion.ShouldNotBeNull();
                bookAgain.Promotion.PromotionalText.ShouldEqual(newText);
            }
        }
    }
}