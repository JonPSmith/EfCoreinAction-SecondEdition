// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Test.Chapter12Listings.IntegrationEventEfClasses
{
    public class LineItem
    {
        public int LineItemId { get; set; }
        public string ProductCode { get; set; }
        public int Amount { get; set; }

        // relationships

        public int OrderId { get; set; }

        public Product Product { get; set; }

    }
}