// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace Test.Chapter11Listings.EfClasses
{
    public class MyEntityGuid
    {
        public Guid Id { get; set; }

        public string MyString { get; set; }

        public OneEntityOptionalGuid OneToOneOptional { get; set; }
    }
}