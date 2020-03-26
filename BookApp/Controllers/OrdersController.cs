// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using BookApp.Controllers;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.OrderServices.Concrete;

namespace EfCoreInAction.Controllers
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
            return View(listService.GetUsersOrders(HttpContext.Request.Cookies));
        }

        public IActionResult ConfirmOrder(int orderId)
        {
            var detailService = new DisplayOrdersService(_context);
            SetupTraceInfo();
            return View(detailService.GetOrderDetail(orderId));
        }
    }
}