// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Domain.Orders;
using BookApp.Domain.Orders.SupportTypes;
using BookApp.Persistence.EfCoreSql.Orders.DbAccess;
using StatusGeneric;

namespace BookApp.BizLogic.Orders.Orders.Concrete
{
    public class PlaceOrderBizLogic : IPlaceOrderBizLogic
    {
        private readonly IPlaceOrderDbAccess _dbAccess;

        public PlaceOrderBizLogic(IPlaceOrderDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        /// <summary>
        /// This validates the input and if OK creates an order and calls the _dbAccess to add to orders
        /// </summary>
        /// <param name="dto">T and Cs, UserId and line items</param>
        /// <returns>returns an Order. Will be null if there are errors</returns>
        public async Task<IStatusGeneric<Order>>   //#A
            CreateOrderAndSaveAsync(PlaceOrderInDto dto) //#B
        {
            var status = new StatusGenericHandler<Order>(); //#C

            if (!dto.AcceptTAndCs)                    //#D
            {
                return status.AddError("You must accept the T&Cs to place an order.");
            }                                         
            if (!dto.LineItems.Any())                 //#D
            {
                return status.AddError("No items in your basket.");
            }                                         

            var booksDict = await _dbAccess                 //#E
                .FindBooksByIdsAsync                        //#E
                     (dto.LineItems.Select(x => x.BookId)); //#E
            
            var linesStatus = FormLineItemsWithErrorChecking //#F
                (dto.LineItems, booksDict);                  //#F
            if (status.CombineStatuses(linesStatus).HasErrors)//#G
                return status;                                //#G

            var orderStatus = Order.CreateOrder(  //#H
                dto.UserId, linesStatus.Result);  //#H

            if (status.CombineStatuses(orderStatus).HasErrors)//#I
                return status;                                //#I
                
            await _dbAccess.AddAndSave(orderStatus.Result); //#J
            
            return status.SetResult(orderStatus.Result); //#K
        }
        /*******************************************************************
        #A This method returns a status with the created Order, which is null if there are no errors
        #B The PlaceOrderInDto contains a TandC bool and a collection of BookIds and number of books 
        #C This status is used to gather and errors and, if no errors, return an Order
        #D These validate the user's input
        #E The _dbAccess contains the code to find each book - see listing 4.3
        #F This method creates list of bookId and number of books - see end of listing 4.2
        #G If any errors were found while checking each order line, then it returns the error status
        #H This calls the Order static factory. It is the Order's job to form the Order with LineItems
        #I Again, any errors will abort the Order and the errors returned
        #J The _dbAccess contains the code add the Order and call SaveChangesAsync
        #K Finally it returns a successful status with the created Order entity
         ****************************************************************/

        private IStatusGeneric<List<OrderBookDto>>  FormLineItemsWithErrorChecking
            (IEnumerable<OrderLineItem> lineItems,            
             IDictionary<int,BookView> booksDict)                 
        {
            var status = new StatusGenericHandler<List<OrderBookDto>>();
            var result = new List<OrderBookDto>();
          
            foreach (var lineItem in lineItems)  
            {
                if (!booksDict.ContainsKey(lineItem.BookId))           
                    throw new InvalidOperationException(
                        $"An order failed because book, id = {lineItem.BookId} was missing.");               

                var bookView = booksDict[lineItem.BookId];
                if (bookView.ActualPrice <= 0)                         
                    status.AddError($"Sorry, the book '{bookView.Title}' is not for sale.");    
                else
                {
                    //Valid, so add to the order
                    result.Add(new OrderBookDto(bookView, lineItem.NumBooks));
                }
            }
            return status.SetResult(result); 
        }
    }
}