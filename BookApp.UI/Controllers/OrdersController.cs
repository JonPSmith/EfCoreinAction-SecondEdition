// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using BookApp.Persistence.EfCoreSql.Orders;
using BookApp.ServiceLayer.EfCoreSql.Orders.OrderServices;
using BookApp.ServiceLayer.EfCoreSql.Orders.OrderServices.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace BookApp.UI.Controllers
{
    public class OrdersController : BaseTraceController
    {
        // GET: /<controller>/
        public IActionResult Index([FromServices] IDisplayOrdersService service)
        {
            var result = service.GetUsersOrders();
            SetupTraceInfo();
            return View(result);
        }

        public IActionResult ConfirmOrder(int orderId, [FromServices] IDisplayOrdersService service)
        {
            var result = service.GetOrderDetail(orderId);
            SetupTraceInfo();
            return View(result);
        }
    }
}