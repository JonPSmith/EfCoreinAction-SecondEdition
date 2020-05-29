// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Test.Chapter06Listings
{
    public class ManyTop
    {
        public int Id { get; set; }

        public IList<Many1> Collection1 { get; set; }
        public IList<Many2> Collection2 { get; set; }
        public IList<Many3> Collection3 { get; set; }
    }
}