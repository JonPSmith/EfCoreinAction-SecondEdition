// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Test.Chapter14Listings
{
    public class MyEntity
    {
        public int Id { get; set; }

        public string MyString { get; set; } = Guid.NewGuid().ToString();

        public ICollection<SubEntity1> Collections { get; set; } = new List<SubEntity1>();

        public HashSet<SubEntity2> HashSets { get; set; } = new HashSet<SubEntity2>();
    }
}