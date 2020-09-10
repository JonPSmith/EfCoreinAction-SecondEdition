// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Test.Chapter12Listings.IntegrationEventEfClasses
{
    public class Order
    {
        public int OrderId { get; set; }
        public Guid UserId { get; set; }

        public ICollection<LineItem> LineItems { get; set; }
    }
}