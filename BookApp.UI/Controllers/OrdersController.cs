// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using BookApp.Persistence.EfCoreSql.Orders;
using BookApp.ServiceLayer.EfCoreSql.Orders.OrderServices.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace BookApp.UI.Controllers
{
    public class OrdersController : BaseTraceController
    {
        private readonly OrderDbContext _context;

        public OrdersController(OrderDbContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var listService = new DisplayOrdersService(_context);
            SetupTraceInfo();
            return View(listService.GetUsersOrders());
        }

        public IActionResult ConfirmOrder(int orderId)
        {
            var detailService = new DisplayOrdersService(_context);
            SetupTraceInfo();
            return View(detailService.GetOrderDetail(orderId));
        }
    }
}