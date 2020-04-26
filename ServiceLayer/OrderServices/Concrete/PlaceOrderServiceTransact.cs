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
    public class PlaceOrderServiceTransact //#A
    {
        private readonly BasketCookie _basketCookie;

        private readonly RunnerTransact2WriteDb
            <PlaceOrderInDto, Part1ToPart2Dto, Order> _runner;

        public PlaceOrderServiceTransact(
            IRequestCookieCollection cookiesIn, 
            IResponseCookies cookiesOut, 
            EfCoreContext context)
        {
            _basketCookie = new BasketCookie(
                cookiesIn, cookiesOut);
            _runner = new RunnerTransact2WriteDb  //#B
                <PlaceOrderInDto, Part1ToPart2Dto, Order>( //#C
                context, //#D
                new PlaceOrderPart1(                 //#E
                    new PlaceOrderDbAccess(context)),//#E
                new PlaceOrderPart2(                   //#F
                    new PlaceOrderDbAccess(context))); //#F
        }

        public IImmutableList<ValidationResult> Errors => _runner.Errors;

        /// <summary>
        /// This creates the order and, if successful clears the cookie
        /// </summary>
        /// <returns>Returns the OrderId, or zero if errors</returns>
        public int PlaceOrder(bool tsAndCsAccepted)
        {
            var checkoutService = new CheckoutCookieService(
                _basketCookie.GetValue());

            var order = _runner.RunAction(
                new PlaceOrderInDto(tsAndCsAccepted, 
                checkoutService.UserId, 
                checkoutService.LineItems));

            if (_runner.HasErrors) return 0;

            //successful so clear the cookie line items
            checkoutService.ClearAllLineItems();
            _basketCookie.AddOrUpdateCookie(
                checkoutService.EncodeForCookie());

            return order.OrderId;
        }
    }
    /*****************************************************
    #A This class is a version of PlaceOrderService, but using transactions to execute the business logic in two parts
    #B I create the BizRunner variant called RunnerTransact2WriteDb, which will handle multiple business logic inside a transaction
    #C The BizRunner needs to know the types of the date used for: input, passing from part 1 to part 2, and output
    #D The BizRunner needs the application's DbContext 
    #E This provides an instance of the first part of the business logic
    #F This provides an instance of the second part of the business logic
     * *******************************************************/
}