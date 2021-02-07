// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using BookApp.BizLogic.Orders.BasketServices;
using BookApp.BizLogic.Orders.Orders;
using BookApp.Persistence.EfCoreSql.Orders;
using BookApp.ServiceLayer.EfCoreSql.Orders.CheckoutServices.Concrete;
using BookApp.ServiceLayer.EfCoreSql.Orders.OrderServices.Concrete;
using BookApp.UI.Controllers;
using GenericServices.AspNetCore;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BookApp.UI.Controllers
{
    public class CheckoutController : BaseTraceController
    {
        private readonly OrderDbContext _context;

        public CheckoutController(OrderDbContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var listService = new CheckoutListService(_context, HttpContext.Request.Cookies);
            var result = listService.GetCheckoutList();
            SetupTraceInfo();
            return View(result);
        }

        public IActionResult Buy(OrderLineItem itemToBuy) 
        {
            var cookie = new BasketCookie(      
                HttpContext.Request.Cookies,    
                HttpContext.Response.Cookies);  
            var service = new CheckoutCookieService(cookie.GetValue());                   
            service.AddLineItem(itemToBuy); 
            var cookieOutString = service.EncodeForCookie();
            cookie.AddOrUpdateCookie(cookieOutString); 
            SetupTraceInfo(); //Remove this when shown in book listing
            return RedirectToAction("Index"); 
        }

        public IActionResult DeleteLineItem(int lineNum)
        {
            var cookie = new BasketCookie(HttpContext.Request.Cookies, HttpContext.Response.Cookies);
            var service = new CheckoutCookieService(cookie.GetValue());
            service.DeleteLineItem(lineNum);
            cookie.AddOrUpdateCookie(service.EncodeForCookie());
            SetupTraceInfo();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> PlaceOrder(bool acceptTAndCs, [FromServices] IPlaceOrderBizLogic bizLogic)
        {
            var service = new PlaceOrderService(HttpContext.Request.Cookies, HttpContext.Response.Cookies, bizLogic);
            var bizStatus = await service.PlaceOrderAndClearBasketAsync(acceptTAndCs);

            if (bizStatus.IsValid)
                return RedirectToAction("ConfirmOrder", "Orders", new {orderId = bizStatus.Result});

            //Otherwise errors, so copy over and redisplay
            bizStatus.CopyErrorsToModelState(ModelState);
            var listService = new CheckoutListService(_context, HttpContext.Request.Cookies);
            SetupTraceInfo();
            return View(listService.GetCheckoutList());
        }
    }
}