// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using Test.Chapter12Listings.IntegrationEventEfClasses;

namespace Test.Chapter12Listings.BusinessLogic
{
    public interface IWarehouseService
    {
        void CheckProductsInStock(List<Order> orders);
    }
}