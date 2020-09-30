// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Chapter08Listings.PropertyBags
{
    public class TestClass
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
    }
}