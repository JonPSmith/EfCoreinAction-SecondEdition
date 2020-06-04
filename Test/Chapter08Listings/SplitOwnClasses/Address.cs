// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Test.Chapter08Listings.SplitOwnClasses
{
    public class Address //#C
    {
        public string NumberAndStreet { get; set; }
        public string City { get; set; }
        public string ZipPostCode { get; set; }
        [Required]
        [MaxLength(2)]
        public string CountryCodeIso2 { get; set; }
    }
    /****************************************************
    #C A owned type has no primary key - this type of class is referred to as a 'Value Object' in Domain-Driven Design terms
    #D Any non-nullable value is stored as a nullable column when the Owned type is added to a class
     * **************************************************/
}