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
    //0123456789|123456789|123456789|123456789|123456789|123456789|123456789|xxxxx!
    /***********************************************************
    #A This class handles the basket cookie which contains the user selected books
    #B This defines the input, PlaceOrderInDto, and output, Order, of this business logic
    #C This holds any errors sent back from the business logic.
    #D The constructor take in to cookie data, plus the application's DbContext
    #E This creates a BasketCookie using the cookie in/out data from ASP.NET Core
    #F This creates the BizRunner, with the business logic, that is to be run
    #G This is the method to call when the user presses the Purchase button
    #H The CheckoutCookieService is a class that encodes/decodes the basket data 
    #I This runs the business logic with the data it needs from the basket cookie
    #J If the business logic has errors  it returns immediately. The basket cookie is not cleared
    #K The order was placed successfully so it clears the basket cookie
    #L It returns the OrderId, which allows ASP.NET confirm the order details to the user
     * *********************************************************/
}