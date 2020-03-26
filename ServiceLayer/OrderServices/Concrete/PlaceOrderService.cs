// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using BizDbAccess.Orders;
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
        private readonly CheckoutCookie _checkoutCookie;  //#A

        private readonly 
            RunnerWriteDb<PlaceOrderInDto, Order> _runner;//#B

        public PlaceOrderService(              //#D
            IRequestCookieCollection cookiesIn,//#D 
            IResponseCookies cookiesOut,       //#D
            EfCoreContext context)             //#D
        {
            _checkoutCookie = new CheckoutCookie(//#E
                cookiesIn, cookiesOut);          //#E
            _runner = 
                new RunnerWriteDb<PlaceOrderInDto, Order>(//#F
                    new PlaceOrderAction(                 //#F
                        new PlaceOrderDbAccess(context)), //#F
                    context);                             //#F
        }

        public IImmutableList<ValidationResult> 
            Errors => _runner.Errors; //#C

        /// <summary>
        /// This creates the order and, if successful clears the cookie
        /// </summary>
        /// <returns>Returns the OrderId, or zero if errors</returns>
        public int PlaceOrder(bool acceptTAndCs) //#G
        {
            var checkoutService = new CheckoutCookieService(//#H
                _checkoutCookie.GetValue());                //#H

            var order = _runner.RunAction(       //#I
                new PlaceOrderInDto(acceptTAndCs,//#I
                checkoutService.UserId,          //#I
                checkoutService.LineItems));     //#I

            if (_runner.HasErrors) return 0; //#J

            //successful so clear the cookie line items
            checkoutService.ClearAllLineItems();   //#K
            _checkoutCookie.AddOrUpdateCookie(     //#K
                checkoutService.EncodeForCookie());//#K

            return order.OrderId;//#L
        }
    }
    /***********************************************************
    #A This is a class that handles the checkout cookie. This is a cookie, but with a specfic name and expiry time
    #B This is the BizRunner that I am going to use to execute the business logic. It is of Type RunnerWriteDb<TIn, TOut>
    #C This holds any errors sent back from the business logic. The caller can use these to redisplay the page and show the errors that need fixing
    #D The constructor needs access to the cookies, both in and out, and the application's DbContext
    #E I create a CheckoutCookie using the cookie in/out access parts from ASP.NET Core
    #F I create the BizRunner, with the business logic, PlaceOrderAction, that I want to run. PlaceOrderAction needs PlaceOrderDbAccess when it is created
    #G This is the method I call from the ASP.NET action that is called when the user presses the Purchase button
    #H The CheckoutCookieService is a class that encodes/decodes the checkout data into a string that goes inside the checkout cookie
    #I I am now ready to run the business logic, handing it the checkout information in the format that it needs
    #J If the business logic has any errors then I return immediately. The checkout cookie has not been cleared so the user can try again
    #K If I get here then the order was placed successfully. I therefore clear the checkout cookie of the order parts
    #L I return the OrderId, that is, the primary key of the order, which ASP.NET uses to show a confirmation page which includes the order details
     * *********************************************************/
}