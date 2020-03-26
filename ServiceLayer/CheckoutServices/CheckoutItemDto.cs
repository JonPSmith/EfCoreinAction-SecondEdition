// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfClasses;

namespace ServiceLayer.CheckoutServices
{
    public class CheckoutItemDto
    {
        public int BookId { get; internal set; }

        public string Title { get; internal set; }

        public string AuthorsName { get; internal set; }

        public decimal BookPrice { get; internal set; }

        public string ImageUrl { get; set; }

        public short NumBooks { get; internal set; }
    }
}