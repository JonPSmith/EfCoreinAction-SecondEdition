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
    public class PlaceOrderServiceWithVal
    {
        private readonly BasketCookie _basketCookie;
        private readonly RunnerWriteDbWithValidation<PlaceOrderInDto, Order> _runner;

        public PlaceOrderServiceWithVal(
            IRequestCookieCollection cookiesIn, 
            IResponseCookies cookiesOut,
            EfCoreContext context)
        {
            _basketCookie = new BasketCookie(cookiesIn, cookiesOut);
            _runner = new RunnerWriteDbWithValidation<PlaceOrderInDto, Order>(
                new PlaceOrderAction(
                    new PlaceOrderDbAccess(context)),
                context);
        }

        public IImmutableList<ValidationResult> Errors => _runner.Errors;

        /// <summary>
        /// This creates the order and, if successful clears the cookie
        /// </summary>
        /// <returns>Returns the OrderId, or zero if errors</returns>
        public int PlaceOrder(bool acceptTAndCs)
        {
            var checkoutService = new CheckoutCookieService(
                _basketCookie.GetValue());

            var order = _runner.RunAction(
                new PlaceOrderInDto(acceptTAndCs, 
                checkoutService.UserId, checkoutService.LineItems));

            if (_runner.HasErrors) return 0;

            //successful so clear the cookie line items
            checkoutService.ClearAllLineItems();
            _basketCookie.AddOrUpdateCookie(
                checkoutService.EncodeForCookie());

            return order.OrderId;
        }
    }
}