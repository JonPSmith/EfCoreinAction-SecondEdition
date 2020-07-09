// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Test.Chapter09Listings.FiveStepMigration
{
    public class Address
    {
        public int AddressId { get; set; }

        public string Street { get; set; }
        public string City { get; set; }
    }
}