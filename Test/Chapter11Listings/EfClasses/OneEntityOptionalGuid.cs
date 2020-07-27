// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace Test.Chapter11Listings.EfClasses
{
    public class OneEntityOptionalGuid
    {
        public Guid Id { get; set; }

        public int MyInt { get; set; }

        public Guid? MyEntityGuidId { get; set; }
    }
}