// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Test.Chapter06Listings
{
    public class BookNotSafe
    {
        public int Id { get; set; }
        public ICollection<ReviewNotSafe> Reviews { get; set; }

        public BookNotSafe()
        {
            Reviews = new List<ReviewNotSafe>();
        }
    }
}