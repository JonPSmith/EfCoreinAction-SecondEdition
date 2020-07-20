// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;

namespace Test.Chapter10Listings.EfClasses
{
    public class DefaultTest
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }

        public DateTime CreatedOn { get; private set; }

        public string OrderId { get; set; }
    }
}
