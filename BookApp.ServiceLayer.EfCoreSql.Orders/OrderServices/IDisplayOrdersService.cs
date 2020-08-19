// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace BookApp.ServiceLayer.EfCoreSql.Orders.OrderServices
{
    public interface IDisplayOrdersService
    {
        /// <summary>
        /// This lists existing orders
        /// </summary>
        /// <returns></returns>
        List<OrderListDto> GetUsersOrders();

        OrderListDto GetOrderDetail(int orderId);
    }
}