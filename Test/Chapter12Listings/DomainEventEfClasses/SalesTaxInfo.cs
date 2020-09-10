// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Test.Chapter12Listings.DomainEventEfClasses
{
    public class SalesTaxInfo
    {
        [Key]
        [MaxLength(20)]
        public string State { get; set; }

        public double SalesTaxPercent { get; set; }
    }
}