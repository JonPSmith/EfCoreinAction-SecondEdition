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
    public class Ch03_Delete
    {
        private readonly ITestOutputHelper _output;

        public Ch03_Delete(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestLoadOnlyBookAndDelete()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var book = context.Books
                .Single(p => p.Title == "Quantum Networking");
            context.Remove(book); 
            context.SaveChanges();

            //VERIFY
            context.Books.Count().ShouldEqual(3);
            context.Set<BookAuthor>().Count().ShouldEqual(3);
            context.Set<Review>().Count().ShouldEqual(0);
        }

        [Fact]
        public void TestDeletePriceOffer()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            var numPromotions = context.PriceOffers.Count();

            //ATTEMPT
            var promotion = context.PriceOffers     //#A
                .First();                           //#A

            context.Remove(promotion);  //#B
            context.SaveChanges();                  //#C                  
            /**********************************************************
                #A I find the first PriceOffer
                #B I then remove that PriceOffer from the application's DbContext. The DbContext works what to remove based on its type of its parameter
                #C The SaveChanges calls DetectChanges which finds a tracked PriceOffer entity which is marked as deleted. It then deletes it from the database
                * *******************************************************/

            //VERIFY
            context.PriceOffers.Count().ShouldEqual(numPromotions - 1);
        }

        [Fact]
        public void TestDeletePriceOfferWithLogging()
        {
            //SETUP
            var showLog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<EfCoreContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.DecodeMessage());
            });
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            var numPromotions = context.PriceOffers.Count();

            //ATTEMPT
            showLog = true;
            var promotion = context.PriceOffers     //#A
                .First();                           //#A

            context.PriceOffers.Remove(promotion);  //#B
            context.SaveChanges();                  //#C                  
            showLog = false;

            //VERIFY
            context.PriceOffers.Count().ShouldEqual(numPromotions - 1);
        }

        [Fact]
        public void TestBookWithRelatedLinks()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            var book = context.Books          
                .Include(p => p.Promotion)    //#A
                .Include(p => p.Reviews)      //#A
                .Include(p => p.AuthorsLink)  //#A
                .Single(p => p.Title          //#B
                             == "Quantum Networking");//#B

            context.Books.Remove(book);       //#C
            context.SaveChanges();            //#D
            /**********************************************************
                #A The three Includes make sure that the three dependent relationships are loaded with the Book
                #B This finds the "Quantum Networking" book, which I know has a promotion, 2 reviews and one BookAuthor link
                #B I then delete that book
                #C The SaveChanges calls DetectChanges which finds a tracked Book entity which is marked as deleted. It then deletes its dependent relationships and then deletes the book
                * *******************************************************/

            //VERIFY
            context.Books.Count().ShouldEqual(3);
            context.PriceOffers.Count().ShouldEqual(0);   //Quantum Networking is the only book with a priceOffer and reviews
            context.Set<Review>().Count().ShouldEqual(0);
            context.Set<BookAuthor>().Count().ShouldEqual(3); //three books left, each with a one author
        }

        [Fact]
        public void TestBookWithRelatedLinksWithoutIncludes()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            var book = context.Books
                .Single(p => p.Title 
                             == "Quantum Networking");

            context.Books.Remove(book); 
            context.SaveChanges();                 

            //VERIFY
            context.Books.Count().ShouldEqual(3);
            context.PriceOffers.Count().ShouldEqual(0);   //Quantum Networking is the only book with a priceOffer and reviews
            context.Set<Review>().Count().ShouldEqual(0);
            context.Set<BookAuthor>().Count().ShouldEqual(3); //three books left, each with a one author
        }
    }
}