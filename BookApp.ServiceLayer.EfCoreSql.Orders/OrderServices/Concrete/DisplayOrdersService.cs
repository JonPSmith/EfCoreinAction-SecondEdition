// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using BookApp.Domain.Orders;
using BookApp.Persistence.EfCoreSql.Orders;
using BookApp.ServiceLayer.EfCoreSql.Orders.CheckoutServices;

namespace BookApp.ServiceLayer.EfCoreSql.Orders.OrderServices.Concrete
{
    public class DisplayOrdersService : IDisplayOrdersService
    {
        private readonly OrderDbContext _context;

        public DisplayOrdersService(OrderDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This lists existing orders
        /// </summary>
        /// <returns></returns>
        public List<OrderListDto> GetUsersOrders()
        {

            return SelectQuery(_context.Orders.OrderByDescending(x => x.DateOrderedUtc))
                .ToList();
        }


        public OrderListDto GetOrderDetail(int orderId)
        {
            var order = SelectQuery(_context.Orders).SingleOrDefault(x => x.OrderId == orderId);

            if (order == null)
                throw new NullReferenceException($"Could not find the order with id of {orderId}.");

            return order;
        }

        //---------------------------------------------
        //private methods

        private IQueryable<OrderListDto> SelectQuery(IQueryable<Order> orders)
        {
            return orders.Select(x => new OrderListDto
                               {
                                   OrderId = x.OrderId,
                                   DateOrderedUtc = x.DateOrderedUtc,
                                   LineItems = x.LineItems.Select(lineItem => new CheckoutItemDto
                                   {
                                       BookId = lineItem.BookId,
                                       Title = lineItem.BookView.Title,
                                       ImageUrl = lineItem.BookView.ImageUrl,
                                       AuthorsName = lineItem.BookView.AuthorsOrdered,
                                       BookPrice = lineItem.BookPrice,
                                       NumBooks = lineItem.NumBooks,
                                   })
                               });
        }
    }
}