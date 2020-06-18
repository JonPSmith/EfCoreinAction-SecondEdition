// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter09Listings.SeedExample
{
    [Owned]
    public class SimpleAddress
    {
        public string Street { get; set; }
        public string City { get; set; }

        public override string ToString()
        {
            return $"{nameof(Street)}: {Street}, {nameof(City)}: {City}";
        }
    }
}