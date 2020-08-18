// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Domain.Orders;
using BookApp.Domain.Orders.SupportTypes;
using BookApp.Persistence.EfCoreSql.Orders.DbAccess.Orders;
using StatusGeneric;

namespace BookApp.Infrastructure.Orders.BizLogic.Orders.Concrete
{
    public class PlaceOrderAction : IPlaceOrderAction
    {
        private readonly IPlaceOrderDbAccess _dbAccess;

        public PlaceOrderAction(IPlaceOrderDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        /// <summary>
        /// This validates the input and if OK creates an order and calls the _dbAccess to add to orders
        /// </summary>
        /// <param name="dto">T and Cs, UserId and line items</param>
        /// <returns>returns an Order. Will be null if there are errors</returns>
        public async Task<IStatusGeneric<Order>> ActionAsync(PlaceOrderInDto dto) 
        {
            var status = new StatusGenericHandler<Order>();

            if (!dto.AcceptTAndCs)                    
            {
                return status.AddError("You must accept the T&Cs to place an order.");
            }                                         
            if (!dto.LineItems.Any())                 
            {
                return status.AddError("No items in your basket.");
            }                                         

            var booksDict = await _dbAccess.FindBooksByIdsAsync    
                     (dto.LineItems.Select(x => x.BookId));
            var linesStatus = FormLineItemsWithErrorChecking(dto.LineItems, booksDict);
            if (status.CombineStatuses(linesStatus).HasErrors)
                return status;

            var orderStatus = Order.CreateOrder(dto.UserId, linesStatus.Result);

            if (status.CombineStatuses(orderStatus).HasErrors)
                return status;
                
            await _dbAccess.AddAndSave(orderStatus.Result);
            
            return status.SetResult(orderStatus.Result);
        }

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
                    result.Add(new OrderBookDto(bookView.BookId, bookView.ActualPrice, lineItem.NumBooks));
                }
            }
            return status.SetResult(result); 
        }
    }
}