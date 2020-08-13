// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using BookApp.Domain.Order;
using DataLayer.Interfaces;
using StatusGeneric;

namespace DataLayer.EfClasses
{
    public class Order : IUserId
    {
        private HashSet<LineItem> _lineItems;

        private Order() { }

        public int OrderId { get; private set; }

        public Guid UserId { get; private set; }
        public DateTime DateOrderedUtc { get; private set; }

        // relationships

        public IEnumerable<LineItem> LineItems => _lineItems?.ToList();


        public static IStatusGeneric<Order> CreateOrder(Guid userId,
            IEnumerable<OrderBookDto> bookOrders)
        {
            var status = new StatusGenericHandler<Order>();
            var order = new Order
            {
                UserId = userId,
                DateOrderedUtc = DateTime.UtcNow
            };

            byte lineNum = 1;
            order._lineItems = new HashSet<LineItem>(bookOrders
                .Select(x => new LineItem( x, lineNum++)));
            if (!order._lineItems.Any())
                status.AddError("No items in your basket.");

            return status.SetResult(order); //don't worry, the Result will return default(T) if there are errors
        }

    }
}