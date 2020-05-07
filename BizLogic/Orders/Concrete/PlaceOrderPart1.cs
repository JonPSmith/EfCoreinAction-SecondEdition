// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BizDbAccess.Orders;
using BizLogic.GenericInterfaces;
using DataLayer.EfClasses;

namespace BizLogic.Orders.Concrete
{
    public class PlaceOrderPart1 : BizActionErrors, IPlaceOrderPart1
    {
        private readonly IPlaceOrderDbAccess _dbAccess;

        public PlaceOrderPart1(IPlaceOrderDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public Part1ToPart2Dto Action(PlaceOrderInDto dto)           
        {
            if (!dto.AcceptTAndCs)
                AddError("You must accept the T&Cs to place an order.");

            if (!dto.LineItems.Any())
                AddError("No items in your basket.");

            var order = new Order                          
            {                                              
                CustomerId = dto.UserId      
            };                                             

            if (!HasErrors)                                
                _dbAccess.Add(order);

            return new Part1ToPart2Dto(dto.LineItems, order);
        }
    }
}