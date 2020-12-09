// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BizLogic.Orders;
using BizLogic.Orders.Concrete;
using Test.Mocks;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestBizLogic
{
    public class Ch04_PlaceOrderAction
    {

        [Fact]
        public void ExampleOfMockingOk()
        {
            //SETUP                                           //#A
            var lineItems = new List<OrderLineItem>           //#A
            {                                                 //#A
                new OrderLineItem {BookId = 1, NumBooks = 4}, //#A
            };                                                //#A
            var userId = Guid.NewGuid();                      //#A
            var input = new PlaceOrderInDto(true, userId,     //#A
                lineItems.ToImmutableList());                 //#A

            var mockDbA = new MockPlaceOrderDbAccess();  //#B
            var service = new PlaceOrderAction(mockDbA); //#C

            //ATTEMPT
            service.Action(input); //#D

            //VERIFY
            service.Errors.Any().ShouldEqual(false); //#
            mockDbA.AddedOrder.CustomerId     //#F
                .ShouldEqual(userId);         //#F
        }
        /****************************************************************************
        #A Creates the input to the PlaceOrderAction method
        #B Creates an instance of the mock database access code. This has numerous controls, but in this case, you use the default settings.
        #C Creates your PlaceOrderAction instance, providing it with a mock of the database access code
        #D Runs the PlaceOrderAction’s method called Action, which takes in the input data and outputs an order
        #E Checks that the order placement completed successfully
        #F Your mock database access code has captured the order that the PlaceOrderAction’s method “wrote” to the database so you can check it was formed properly.
         ******************************************************************************/

        [Fact]
        public void BookNotForSale()
        {
            //SETUP
            var mockDbA = new MockPlaceOrderDbAccess(false, -1);
            var service = new PlaceOrderAction(mockDbA);
            var lineItems = new List<OrderLineItem>
            {
                new OrderLineItem {BookId = 1, NumBooks = 1},
            };
            var userId = Guid.NewGuid();

            //ATTEMPT
            service.Action(new PlaceOrderInDto(true, userId, lineItems.ToImmutableList()));

            //VERIFY
            service.Errors.Any().ShouldEqual(true);
            service.Errors.Count.ShouldEqual(1);
            service.Errors.First().ErrorMessage.ShouldEqual("Sorry, the book 'Book0000 Title' is not for sale.");
        }

        //------------------------------------------------
        //failure mode

        [Fact]
        public void MissingBookError()
        {
            //SETUP
            var mockDbA = new MockPlaceOrderDbAccess();
            var service = new PlaceOrderAction(mockDbA);
            var lineItems = new List<OrderLineItem>
            {
                new OrderLineItem {BookId = 1, NumBooks = 4},
                new OrderLineItem {BookId = 1000, NumBooks = 5},
                new OrderLineItem {BookId = 3, NumBooks = 6}
            };
            var userId = Guid.NewGuid();

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>( 
                () => service.Action(new PlaceOrderInDto(true, userId, lineItems.ToImmutableList())));

            //VERIFY
            ex.Message.ShouldEqual("An order failed because book, id = 1000 was missing.");
        }

        [Fact]
        public void NotAcceptTsAndCs()
        {
            //SETUP
            var mockDbA = new MockPlaceOrderDbAccess(true);
            var service = new PlaceOrderAction(mockDbA);
            var userId = Guid.NewGuid();

            //ATTEMPT
            service.Action(new PlaceOrderInDto(false, userId, null));

            //VERIFY
            service.Errors.Any().ShouldEqual(true);
            service.Errors.Count.ShouldEqual(1);
            service.Errors.First().ErrorMessage.ShouldEqual("You must accept the T&Cs to place an order.");
        }

        [Fact]
        public void PlaceOrderOk()
        {
            //SETUP
            var mockDbA = new MockPlaceOrderDbAccess();
            var service = new PlaceOrderAction(mockDbA);
            var lineItems = new List<OrderLineItem>
            {
                new OrderLineItem {BookId = 1, NumBooks = 4},
                new OrderLineItem {BookId = 2, NumBooks = 5},
                new OrderLineItem {BookId = 3, NumBooks = 6}
            };
            var userId = Guid.NewGuid();

            //ATTEMPT
            var result = service.Action(new PlaceOrderInDto(true, userId, lineItems.ToImmutableList()));

            //VERIFY
            service.Errors.Any().ShouldEqual(false);
            mockDbA.AddedOrder.CustomerId.ShouldEqual(userId);
            mockDbA.AddedOrder.DateOrderedUtc.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-1,0);
            mockDbA.AddedOrder.LineItems.Count.ShouldEqual(lineItems.Count);
            var orderLineItems = mockDbA.AddedOrder.LineItems.ToImmutableList();
            for (int i = 0; i < lineItems.Count; i++)
            {
                orderLineItems[i].LineNum.ShouldEqual((byte)(i+1));
                orderLineItems[i].ChosenBook.BookId.ShouldEqual(lineItems[i].BookId);
                orderLineItems[i].NumBooks.ShouldEqual(lineItems[i].NumBooks);
            }
        }

        [Fact]
        public void PlaceOrderWithPromotionOk()
        {
            //SETUP
            var mockDbA = new MockPlaceOrderDbAccess(false, 999);
            var service = new PlaceOrderAction(mockDbA);
            var lineItems = new List<OrderLineItem>
            {
                new OrderLineItem {BookId = 1, NumBooks = 1},
                new OrderLineItem {BookId = 2, NumBooks = 1},
            };
            var userId = Guid.NewGuid();

            //ATTEMPT
            var result = service.Action(new PlaceOrderInDto(true, userId, lineItems.ToImmutableList()));

            //VERIFY
            service.Errors.Any().ShouldEqual(false);

            var orderLineItems = mockDbA.AddedOrder.LineItems.ToList();
            orderLineItems.First().BookPrice.ShouldEqual(999);
            orderLineItems.Last().BookPrice.ShouldEqual(2);

        }
    }
}