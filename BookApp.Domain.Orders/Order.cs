// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using BookApp.Domain.Orders.SupportTypes;
using StatusGeneric;

namespace BookApp.Domain.Orders
{
    public class Order : IUserId
    {
        private HashSet<LineItem> _lineItems;

        private Order() { }

        public int OrderId { get; private set; }

        public Guid UserId { get; private set; }
        public DateTime DateOrderedUtc { get; private set; }

        // relationships

        public IReadOnlyCollection<LineItem> LineItems => _lineItems?.ToList();

        public static IStatusGeneric<Order> CreateOrder  //#A
            (Guid userId,  //#B
            IEnumerable<OrderBookDto> bookOrders) //#C
        {
            var status = new StatusGenericHandler<Order>(); //#D
            var order = new Order                 //#E
            {                                     //#E
                UserId = userId,                  //#E
                DateOrderedUtc = DateTime.UtcNow  //#E
            };                                    //#E

            byte lineNum = 1;
            order._lineItems = new HashSet<LineItem>(        //#F
                bookOrders                                   //#F
                .Select(x => new LineItem( x, lineNum++)));  //#F

            if (!order._lineItems.Any())                     //#G
                status.AddError("No items in your basket."); //#G

            return status.SetResult(order); //#H
        }
        /***************************************************************
        #A This static factory will create the Order with lineItems
        #B The Order uses the UserId to only show orders to the person who created it
        #C The OrderBookDto live the Order domain and carries the info the Order needs
        #D Create a status to return with an optional result of Order
        #E This sets the standard properties in an order
        #F This creates each of the LineItems in the same order the user added them
        #G This is a double-check that the Order is valid.
        #H This returns the status with the Order. If there are errors the status sets the result to null
         ***************************************************************/

    }
}