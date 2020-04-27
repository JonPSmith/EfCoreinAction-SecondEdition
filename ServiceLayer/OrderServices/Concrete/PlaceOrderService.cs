// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using BizDbAccess.Orders;
using BizLogic.BasketServices;
using BizLogic.Orders;
using BizLogic.Orders.Concrete;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Http;
using ServiceLayer.BizRunners;
using ServiceLayer.CheckoutServices.Concrete;

namespace ServiceLayer.OrderServices.Concrete
{
    public class PlaceOrderService
    {
        private readonly BasketCookie _basketCookie;        //#A

        private readonly                                    //#B
            RunnerWriteDb<PlaceOrderInDto, Order> _runner;  //#B
        public IImmutableList<ValidationResult>             //#C
            Errors => _runner.Errors;                       //#C

        public PlaceOrderService(                           //#D
            IRequestCookieCollection cookiesIn,             //#D 
            IResponseCookies cookiesOut,                    //#D
            EfCoreContext context)                          //#D
        {
            _basketCookie = new BasketCookie(               //#E
                cookiesIn, cookiesOut);                     //#E
            _runner = 
                new RunnerWriteDb<PlaceOrderInDto, Order>(  //#F
                    new PlaceOrderAction(                   //#F
                        new PlaceOrderDbAccess(context)),   //#F
                    context);                               //#F
        }

        /// <summary>
        /// This creates the order and, if successful clears the cookie
        /// </summary>
        /// <returns>Returns the OrderId, or zero if errors</returns>
        public int PlaceOrder(bool acceptTAndCs)            //#G
        {
            var checkoutService = new CheckoutCookieService(//#H
                _basketCookie.GetValue());                  //#H

            var order = _runner.RunAction(                  //#I
                new PlaceOrderInDto(acceptTAndCs,           //#I
                checkoutService.UserId,                     //#I
                checkoutService.LineItems));                //#I

            if (_runner.HasErrors) return 0;                //#J

            //successful so clear the cookie line items
            checkoutService.ClearAllLineItems();            //#K
            _basketCookie.AddOrUpdateCookie(                //#K
                checkoutService.EncodeForCookie());         //#K

            return order.OrderId;                           //#L
        }
    }
    /***********************************************************
    #A This is a class that handles the basket cookie. This is a cookie, but with a specific name and expiry time
    #B This is the BizRunner that I am going to use to execute the business logic. It is of Type RunnerWriteDb<TIn, TOut>
    #C This holds any errors sent back from the business logic. The caller can use these to redisplay the page and show the errors that need fixing
    #D The constructor needs access to the cookies, both in and out, and the application's DbContext
    #E I create a BasketCookie using the cookie in/out access parts from ASP.NET Core
    #F I create the BizRunner, with the business logic, PlaceOrderAction, that I want to run. PlaceOrderAction needs PlaceOrderDbAccess when it is created
    #G This is the method I call from the ASP.NET action that is called when the user presses the Purchase button
    #H The CheckoutCookieService is a class that encodes/decodes the basket data into a string that goes inside the basket cookie
    #I I am now ready to run the business logic, handing it the basket information in the format that it needs
    #J If the business logic has any errors then it returns immediately. The basket cookie has not been cleared so the user can try again
    #K If I get here then the order was placed successfully. I therefore clear the basket cookie of the order parts
    #L I return the OrderId, that is, the primary key of the order, which ASP.NET uses to show a confirmation page which includes the order details
     * *********************************************************/
}