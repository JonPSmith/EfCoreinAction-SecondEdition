// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Test.Chapter08Listings.SplitOwnClasses
{
    public class OrderInfo //#A
    {
        public int OrderInfoId { get; set; }
        public string OrderNumber { get; set; }

        public Address BillingAddress { get; set; } //#B
        public Address DeliveryAddress { get; set; } //#B
    }


    /**********************************************************
    #A The entity class OrderInfo, with a primary key and two addresses
    #B There are two, distinct Address classes. The data for each address class will be included in the table that the OrderInfo is mapped to
     * *********************************************************/
}