// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter12Listings.IntegrationEventEfClasses;

namespace Test.Chapter12Listings.BusinessLogic
{
    public class WarehouseEventHandler : IWarehouseEventHandler
    {
        private Order _order;

        public bool NeedsCallToWarehouse(DbContext context)   //#A
        {
            var newOrders = context.ChangeTracker             //#B
                .Entries<Order>()                             //#B
                .Where(x => x.State == EntityState.Added)     //#B
                .Select(x => x.Entity)                        //#B
                .ToList();                                    //#B

            if (newOrders.Count > 1)                          //#C
                throw new Exception(                          //#C
                    "Can only process one Order at a time");  //#C

            if (!newOrders.Any())                             //#D 
                return false;                                 //#D

            _order = newOrders.Single();                      //#E
            return true;                                      //#E
        }

        public List<string> AllocateOrderAndDispatch()        //#F
        {
            var errors = new List<string>();

            //... code to communicate with warehouse          //#G
            var prefix = $"Order {_order.OrderId}: ";
            foreach (var lineItem in _order.LineItems)
            {
                if (lineItem.ProductCode.Contains("2x8"))
                    errors.Add($"{prefix}" +
                               $"We are out of stock of {lineItem.ProductCode}");
                if (lineItem.ProductCode == "B1x2Blue"
                    && lineItem.Amount > 20)
                    errors.Add($"{prefix}" +
                               $"We only have 20 {lineItem.ProductCode} in stock");
            }

            return errors;                                    //#H
        }
    }
    /************************************************************
    #A This method detects the event. It returns true if there is an Order to send to the warehouse
    #B This obtains all the newly created Orders
    #C The business logic only handles one Order per SaveChanges call
    #D If there isn't a new Order, then it returns false
    #E If there is an Order it retains it and returns true
    #F This method will communicate with the warehouse. It returns any errors the warehouse sends back
    #G This is where you add the code to communicate with the warehouse
    #H It returns a list of errors. If the list is empty then it was successful
     **********************************************************/
}