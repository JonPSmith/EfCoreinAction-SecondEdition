// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch04_CascadeDelete
    {
        [Fact]
        public void TestDeleteBookNoOrder()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT

            context.Books.Remove(context.Books.First());
            context.SaveChanges();

            //VERIFY
            context.Books.Count().ShouldEqual(3);
        }

        [Fact]
        public void TestDeleteBookInLineItemFails()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();
            var userId = Guid.NewGuid();

            var order = new Order
            {
                CustomerId = userId,
                LineItems = new List<LineItem>
                {
                    new LineItem
                    {
                        ChosenBook = context.Books.First(),
                        LineNum = 0,
                        BookPrice = 123,
                        NumBooks = 1
                    }
                }
            };
            context.Orders.Add(order);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //ATTEMPT
            context.Books.Remove(context.Books.First());
            var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

            //VERIFY
            ex.InnerException.Message.ShouldEqual("SQLite Error 19: 'FOREIGN KEY constraint failed'.");
        }

        [Fact]
        public void TestDeleteBookInLineItemFailsWithEfNotKnowingAboutTheOrder()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureClean();
            context.SeedDatabaseFourBooks();

            var userId = Guid.NewGuid();

            var order = new Order
            {
                CustomerId = userId,
                LineItems = new List<LineItem>
                {
                    new LineItem
                    {
                        ChosenBook = context.Books.First(),
                        LineNum = 0,
                        BookPrice = 123,
                        NumBooks = 1
                    }
                }
            };
            context.Orders.Add(order);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //ATTEMPT
            context.Books.Remove(context.Books.First());
            var ex = Assert.ThrowsAny<DbUpdateException>(() => context.SaveChanges());

            //VERIFY
            ex.InnerException.Message.StartsWith("The DELETE statement conflicted with the REFERENCE constraint \"FK_LineItem_Books_BookId\". ")
                .ShouldBeTrue();
        }
    }
}