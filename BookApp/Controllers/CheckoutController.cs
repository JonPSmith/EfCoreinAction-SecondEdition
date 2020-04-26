// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BizLogic.BasketServices;
using BizLogic.Orders;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.CheckoutServices.Concrete;
using ServiceLayer.OrderServices.Concrete;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BookApp.Controllers
{
    public class CheckoutController : BaseTraceController
    {
        private readonly EfCoreContext _context;

        public CheckoutController(EfCoreContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var listService = new CheckoutListService(_context, HttpContext.Request.Cookies);
            SetupTraceInfo();
            return View(listService.GetCheckoutList());
        }

        public IActionResult Buy(OrderLineItem itemToBuy) //#A
        {
            var cookie = new BasketCookie(      //#B
                HttpContext.Request.Cookies,    //#B
                HttpContext.Response.Cookies);  //#B
            var service = new CheckoutCookieService(  //#C
                cookie.GetValue());                   //#C
            service.AddLineItem(itemToBuy); //#D
            var cookieOutString = service.EncodeForCookie(); //E
            cookie.AddOrUpdateCookie(cookieOutString); //#F
            SetupTraceInfo(); //Remove this when shown in book listing
            return RedirectToAction("Index"); //#G
        }
        /*********************************************************
        #A This action is called if a user clicks any of the  "Buy n books" dropdown buttons.
        #B To isolate the CheckoutCookieService from the ASP.NET Core's HttpContext we have designed a BasketCookie that provides an interface that can be used in a unit test
        #C The CheckoutCookieService is created, with the content of the current basket Cookie, or null if no basket Cookie currently exists
        #D This method adds a new OrderLineItem entry to the CheckoutCookieService list of OrderLineItems 
        #E This statement encodes the list of OrderLineItems into a string, ready to go out in the updated basket Cookie 
        #F This method sets the basket cookie content to the encoded string
        #G Finally we go to the Checkout page which shows the user the books, quantities and prices of the books they have in their basket
         * ******************************************************/


        public IActionResult DeleteLineItem(int lineNum)
        {
            var cookie = new BasketCookie(HttpContext.Request.Cookies, HttpContext.Response.Cookies);
            var service = new CheckoutCookieService(cookie.GetValue());
            service.DeleteLineItem(lineNum);
            cookie.AddOrUpdateCookie(service.EncodeForCookie());
            SetupTraceInfo();
            return RedirectToAction("Index");
        }

        public IActionResult PlaceOrder(bool acceptTAndCs)
        {
            var service = new PlaceOrderService(HttpContext.Request.Cookies, HttpContext.Response.Cookies, _context);
            var orderId = service.PlaceOrder(acceptTAndCs);

            if (!service.Errors.Any())
                return RedirectToAction("ConfirmOrder", "Orders", new {orderId});

            //Otherwise errors, so copy over and redisplay
            foreach (var error in service.Errors)
            {
                var properties = error.MemberNames.ToList();
                ModelState.AddModelError(properties.Any() ? properties.First() : "", error.ErrorMessage);
            }

            var listService = new CheckoutListService(_context, HttpContext.Request.Cookies);
            SetupTraceInfo();
            return View(listService.GetCheckoutList());
        }
    }
}