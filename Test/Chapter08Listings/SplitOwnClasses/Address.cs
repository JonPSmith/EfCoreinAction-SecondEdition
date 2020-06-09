// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter08Listings.SplitOwnClasses
{
    [Owned]                                         //#C
    public class Address                            //#D
    {
        public string NumberAndStreet { get; set; }
        public string City { get; set; }
        public string ZipPostCode { get; set; }
        [Required]                                  //#E
        [MaxLength(2)]                              //#E
        public string CountryCodeIso2 { get; set; } //#E
    }
    /****************************************************
    #C The attribute [Owned] tells EF Core that it is an owned type
    #D An owned type has no primary key
    #E Any non-nullable value is stored as a nullable column when the Owned type is added to a class
     * **************************************************/
}