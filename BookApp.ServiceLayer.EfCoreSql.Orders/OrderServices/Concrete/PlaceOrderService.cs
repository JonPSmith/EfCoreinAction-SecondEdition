// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading.Tasks;
using BookApp.BizLogic.Orders.BasketServices;
using BookApp.BizLogic.Orders.Orders;
using BookApp.ServiceLayer.EfCoreSql.Orders.CheckoutServices.Concrete;
using Microsoft.AspNetCore.Http;
using StatusGeneric;

namespace BookApp.ServiceLayer.EfCoreSql.Orders.OrderServices.Concrete
{
    //NOTE: this service is build this way to allow unit testing. Passing HttpAccessor would be easier, but very hard to test
    public class PlaceOrderService
    {
        private readonly BasketCookie _basketCookie;
        private readonly IPlaceOrderBizLogic _placeOrder;

        public PlaceOrderService(                           
            IRequestCookieCollection cookiesIn,              
            IResponseCookies cookiesOut,                    
            IPlaceOrderBizLogic placeOrder)                          
        {
            _basketCookie = new BasketCookie(cookiesIn, cookiesOut);
            _placeOrder = placeOrder;
        }

        /// <summary>
        /// This creates the order and, if successful clears the cookie
        /// </summary>
        /// <returns>Returns the OrderId, or zero if errors</returns>
        public async Task<IStatusGeneric<int>> PlaceOrderAndClearBasketAsync(bool acceptTAndCs)            
        {
            var status = new StatusGenericHandler<int>();

            var checkoutService = new CheckoutCookieService(
                _basketCookie.GetValue());                  

            var bizStatus = await _placeOrder.CreateOrderAndSaveAsync(                  
                new PlaceOrderInDto(acceptTAndCs, checkoutService.UserId, checkoutService.LineItems));


            if (status.CombineStatuses(bizStatus).HasErrors)
                return status;

            //successful so clear the cookie line items
            checkoutService.ClearAllLineItems();            
            _basketCookie.AddOrUpdateCookie(                
                checkoutService.EncodeForCookie());         

            return status.SetResult( bizStatus.Result.OrderId);                           
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