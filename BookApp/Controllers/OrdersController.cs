// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfCode;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.OrderServices.Concrete;

namespace BookApp.Controllers
{
    public class OrdersController : BaseTraceController
    {
        private readonly EfCoreContext _context;

        public OrdersController(EfCoreContext context)
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