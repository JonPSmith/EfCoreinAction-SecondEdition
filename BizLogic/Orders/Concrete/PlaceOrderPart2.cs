// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BizDbAccess.Orders;
using BizLogic.GenericInterfaces;
using DataLayer.EfClasses;

namespace BizLogic.Orders.Concrete
{
    public class PlaceOrderPart2 : BizActionErrors, IPlaceOrderPart2
    {
        private readonly IPlaceOrderDbAccess _dbAccess;


        public PlaceOrderPart2(IPlaceOrderDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public Order Action(Part1ToPart2Dto dto)           
        {                                            

            var booksDict =                                
                _dbAccess.FindBooksByIdsWithPriceOffers    
                     (dto.LineItems.Select(x => x.BookId));
            dto.Order.LineItems =
                FormLineItemsWithErrorChecking(dto.LineItems, booksDict);                          

            return HasErrors ? null : dto.Order;               
        }

        private List<LineItem>  FormLineItemsWithErrorChecking//#I
            (IEnumerable<OrderLineItem> lineItems,            //#I
             IDictionary<int,Book> booksDict)                 //#I
        {
            var result = new List<LineItem>();
            var i = 1;
            
            foreach (var lineItem in lineItems)             
            {
                if (!booksDict.                             
                    ContainsKey(lineItem.BookId))           
                        throw new InvalidOperationException 
    ($"Could not find the {i} book you wanted to order." +  
     " Please remove that book and try again.");            

                var book = booksDict[lineItem.BookId];
                var bookPrice = 
                    book.Promotion?.NewPrice ?? book.Price; 
                if (book.PublishedOn > DateTime.UtcNow)     
                    AddError(                            
    $"Sorry, the book '{book.Title}' is not yet in print.");
                else if (bookPrice <= 0)                    
                    AddError(                            
    $"Sorry, the book '{book.Title}' is not for sale.");    
                else
                {
                    //Valid, so add to the order
                    result.Add(new LineItem                 
                    {                                       
                        BookPrice = bookPrice,              
                        ChosenBook = book,                  
                        LineNum = (byte)(i++),              
                        NumBooks = lineItem.NumBooks        
                    });
                }
            }
            return result;                                  
        }
    }
}