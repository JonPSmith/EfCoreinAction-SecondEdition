// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading.Tasks;
using BookApp.Domain.Orders;
using StatusGeneric;

namespace BookApp.BizLogic.Orders.Orders
{
    public interface IPlaceOrderBizLogic
    {
        Task<IStatusGeneric<Order>> CreateOrderAndSaveAsync(PlaceOrderInDto dto);
    }
}