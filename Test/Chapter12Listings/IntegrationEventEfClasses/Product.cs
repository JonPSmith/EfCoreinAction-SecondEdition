// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Test.Chapter12Listings.IntegrationEventEfClasses
{
    public class Product
    {
        [Key]
        [MaxLength(20)]
        public string ProductCode { get; set; }

        public string Name { get; set; }
    }
}