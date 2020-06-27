// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using DataLayer.Interfaces;

namespace DataLayer.EfClasses
{
    public class Order : IUserId
    {
        public Order()
        {
            DateOrderedUtc = DateTime.UtcNow;
        }

        public int OrderId { get; set; }

        public DateTime DateOrderedUtc { get; set; }

        // relationships

        public ICollection<LineItem> LineItems { get; set; }

        // Extra columns not used by EF

        public string OrderNumber => $"SO{OrderId:D6}";

        public Guid UserId { get; set; }
    }
}