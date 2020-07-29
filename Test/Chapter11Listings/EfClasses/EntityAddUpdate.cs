// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Reflection.Metadata.Ecma335;

namespace Test.Chapter11Listings.EfClasses
{
    public class EntityAddUpdate : CreatedUpdatedInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}