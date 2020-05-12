// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

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
    public class Ch06_CopyEntityWithRelationships
    {
        [Fact]
        public void TestCopyOrderOk()
        {
            //SETUP
            var sqlOptions = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(sqlOptions))
            {
                context.Database.EnsureCreated();

                var books = context.SeedDatabaseFourBooks();                 //#A
                var order = new Order                                        //#B
                {
                    CustomerId = Guid.Empty,                                 //#C
                    LineItems = new List<LineItem>
                    {
                        new LineItem                                         //#D
                        {                                                    //#D
                            LineNum = 1, ChosenBook = books[0], NumBooks = 1 //#D
                        },                                                   //#D
                        new LineItem                                         //#E
                        {                                                    //#E
                            LineNum = 2, ChosenBook = books[1], NumBooks = 2 //#E
                        },                                                   //#E
                    }
                };
                context.Add(order);                                          //#F
                context.SaveChanges();                                       //#F
                /*************************************************************
                #A For this test I add four books
                #B I create an Order with two LinItems that I want to copy
                #C I set CustomerId to the default value so that the query filter lets me read the order back
                #D I add the first LineNum linked to the first book
                #E I add the second LineNum linked to the second book
                #F I write this Order out to the database
                 **********************************************************/
            }
            using (var context = new EfCoreContext(sqlOptions))
            {
                //ATTEMPT
                var order = context.Orders                   //#A
                    .AsNoTracking()                          //#B
                    .Include(x => x.LineItems)               //#C
                                                             //#D
                    .Single();                               //#E

                order.OrderId = default;                     //#F
                order.LineItems.First().LineItemId = default;//#F
                order.LineItems.Last().LineItemId = default; //#F
                context.Add(order);                          //#G
                context.SaveChanges();                       //#G
                /******************************************************
                #A This is going to query the Orders table.
                #B We want the entities read in as not tracked, that means their State will be Detached
                #C We include the LineItems as we want to copy those too
                #D We do NOT add .ThenInclude(x => x.ChosenBook) to the query. If we did it would copy the Book entities and that not what we want
                #E We take the Order that we want to copy
                #F Now we reset the primary keys (Order and LineItem) to their default value. That will tell the database to generate new primary keys 
                #G Finally we write out the order, which then create a copy.
                 ****************************************************/
            }
            using (var context = new EfCoreContext(sqlOptions))
            {
                //VERIFY
                var orders = context.Orders
                    .Include(x => x.LineItems)
                    .ToList();
                orders.Count.ShouldEqual(2);
                orders[0].LineItems.Count.ShouldEqual(2);
                orders[1].LineItems.Count.ShouldEqual(2);
                for (int i = 0; i < 2; i++)
                {
                    orders[i].LineItems.OrderBy(x => x.LineNum).Select(x => x.BookId).ShouldEqual(new []{1,2});
                    orders[i].LineItems.OrderBy(x => x.LineNum).Select(x => x.NumBooks).ShouldEqual(new short[] { 1, 2 });
                    orders[i].LineItems.OrderBy(x => x.LineNum).Select(x => x.LineNum).ShouldEqual(new byte[] { 1, 2 });
                }
            }
        }
    }
}