// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace DataLayer.EfClasses
{
    public class Order
    {
        public int OrderId { get; set; }

        public DateTime DateOrderedUtc { get; set; }

        /// <summary>
        /// In this simple example the cookie holds a GUID for everyone that 
        /// </summary>
        public Guid CustomerId { get; set; }

        // relationships

        public ICollection<LineItem> LineItems { get; set; }

        // Extra columns not used by EF

        public string OrderNumber => $"SO{OrderId:D6}";

        public Order()
        {
            DateOrderedUtc = DateTime.UtcNow;
        }
    }
}