// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Test.Chapter12Listings.IntegrationEventEfClasses;

namespace Test.Chapter12Listings.BusinessLogic
{
    public class WarehouseService : IWarehouseService
    {
        public List<string> AllocateOrderAndDispatch(Order order)
        {
            var errors = new List<string>();

            var prefix = $"Order {order.OrderId}: ";
            foreach (var lineItem in order.LineItems)
            {
                if (lineItem.ProductCode.Contains("2x8"))
                    errors.Add($"{prefix}" +
                               $"We are out of stock of {lineItem.ProductCode}");
                if (lineItem.ProductCode == "B1x2Blue"
                    && lineItem.Amount > 20)
                    errors.Add($"{prefix}" +
                               $"We only have 20 {lineItem.ProductCode} in stock");
            }

            return errors;

        }
    }
}