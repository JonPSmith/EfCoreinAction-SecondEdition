// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch06_ComplexQueryOperators
    {
        [Fact]
        public void TestLinqAggregatesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var max = context.Books.Max(x => (double?)x.Price);
                var min = context.Books.Min(x => (double?)x.Price);
                var sum = context.Books.Sum(x => (double?)x.Price);
                var avg = context.Books.Average(x => (double?)x.Price);
                var countAverage = context.Books.Count();

                //VERIFY
            }
        }

        [Fact]
        public void TestLinqAggregateOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => 
                    context.Books.Aggregate(0.0, (i, book) => (double)book.Price));

                //VERIFY
                ex.Message.ShouldContain("could not be translated");
            }
        }
    }
}
