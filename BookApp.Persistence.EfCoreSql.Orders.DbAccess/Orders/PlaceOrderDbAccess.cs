// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using BookApp.Domain.Orders;

namespace BookApp.Persistence.EfCoreSql.Orders.DbAccess.Orders
{
    public interface IPlaceOrderDbAccess
    {
        /// <summary>
        /// This finds any books that fits the BookIds given to it
        /// </summary>
        /// <param name="bookIds"></param>
        /// <returns>A dictionary with the BookId as the key, and the Book as the value</returns>
        IDictionary<int, BookView> FindBooksByIds(IEnumerable<int> bookIds);

        void Add(Order newOrder);
    }

    public class PlaceOrderDbAccess : IPlaceOrderDbAccess
    {
        private readonly OrderDbContext _context;

        public PlaceOrderDbAccess(OrderDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This finds any books that fits the BookIds given to it, with any optional promotion
        /// </summary>
        /// <param name="bookIds"></param>
        /// <returns>A dictionary with the BookId as the key, and the Book as the value</returns>
        public IDictionary<int, BookView> FindBooksByIds(IEnumerable<int> bookIds)               
        {
            return _context.BookViews                       
                .Where(x => bookIds.Contains(x.BookId)) 
                .ToDictionary(key => key.BookId);       
        }

        public void Add(Order newOrder)                 
        {                                               
            _context.Orders.Add(newOrder);              
        } 
    }

}