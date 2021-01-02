// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Utils;
using Test.Mocks;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch11_PkResetter
    {
        private readonly Guid _userId = Guid.Empty;

        [Fact]
        public void TestResetPksEntityAndRelationships()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            options.StopNextDispose();
            using (var context = new EfCoreContext(options, new FakeUserIdService(_userId)))
            {
                CreateOrderToCopyInDifferentContext(options);

                var orderNoBook = context.Orders
                    .Include(x => x.LineItems)
                    .AsNoTracking()
                    .Single();

                //ATTEMPT
                var resetter = new PkResetter(context);
                resetter.ResetPksEntityAndRelationships(orderNoBook);

                //VERIFY
                orderNoBook.OrderId.ShouldEqual(0);
                orderNoBook.LineItems.Select(x => x.LineItemId).ShouldEqual(new []{0,0});
            }
        }

        private void CreateOrderToCopyInDifferentContext(DbContextOptions<EfCoreContext> options)
        {
            using (var context = new EfCoreContext(options, new FakeUserIdService(_userId)))
            {
                context.Database.EnsureCreated();

                var books = context.SeedDatabaseFourBooks();
                var order = new Order
                {
                    UserId = _userId,
                    LineItems = new List<LineItem>
                    {
                        new LineItem
                        {
                            LineNum = 1, ChosenBook = books[0], NumBooks = 1
                        },
                        new LineItem
                        {
                            LineNum = 1, ChosenBook = books[1], NumBooks = 2
                        },
                    }
                };
                context.Add(order);
                context.SaveChanges();
            }

        }
    }
}