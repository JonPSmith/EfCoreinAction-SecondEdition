// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Immutable;
using DataLayer.EfClasses;

namespace BizLogic.Orders
{
    public class Part1ToPart2Dto
    {
        public Part1ToPart2Dto(IImmutableList<OrderLineItem> lineItems, Order order)
        {
            LineItems = lineItems;
            Order = order;
        }

        public IImmutableList<OrderLineItem> LineItems { get; private set; }

        public Order Order { get; private set; }
    }
}