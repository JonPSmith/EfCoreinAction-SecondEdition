// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Test.Chapter08Listings.EfClasses
{
    public class SoldIt
    {
        public int SoldItId { get; set; }

        [Required]
        public string WhatSold { get; set; }

        public Payment Payment { get; set; }
        public int PaymentId { get; set; }
    }
}