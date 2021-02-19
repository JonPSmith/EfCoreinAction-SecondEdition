// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using Test.Mocks;
using Test.TestHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestSupportCode
{
    public class Ch04_MockPlaceOrderDbAccess
    {
        [Fact]
        public void TestMockDefaultBooks()
        {
            //SETUP

            //ATTEMPT
            var mock = new StubPlaceOrderDbAccess();
           
            //VERIFY
            mock.Books.Count.ShouldEqual(10);
            mock.Books.ForEach(b => b.Promotion.ShouldBeNull());
            mock.Books.ForEach(b => b.PublishedOn.Year.ShouldEqual(EfTestData.DummyBookStartDate.Year));
        }

        [Fact]
        public void TestMockIncreasingYearBooks()
        {
            //SETUP

            //ATTEMPT
            var mock = new StubPlaceOrderDbAccess(true);

            //VERIFY
            var expectedYear = EfTestData.DummyBookStartDate.Year;
            foreach (var book in mock.Books)
            {
                book.PublishedOn.Year.ShouldEqual(expectedYear++);
            }
            mock.Books.Last().PublishedOn.Year.ShouldEqual(DateTime.Today.Year+1);
        }

        [Fact]
        public void TestMockPromotionOnFirstBooks()
        {
            //SETUP

            //ATTEMPT
            var mock = new StubPlaceOrderDbAccess(false,100);

            //VERIFY
            mock.Books.Count.ShouldEqual(10);
            mock.Books.First().Promotion.ShouldNotBeNull();
            mock.Books.First().Promotion.NewPrice.ShouldEqual(100);
            mock.Books.Skip(1).ToList().ForEach(b => b.Promotion.ShouldBeNull());
        }
    }
}