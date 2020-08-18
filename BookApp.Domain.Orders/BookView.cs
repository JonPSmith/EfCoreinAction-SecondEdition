// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace BookApp.Domain.Orders
{
    //This is mapped to the real Book entity class as a View
    public class BookView
    {
        [Key]
        public int BookId { get; private set; }

        public string Title { get; private set; }

        public string AuthorsOrdered { get; private set; }

        public decimal ActualPrice { get; private set; }

        public string ImageUrl { get; private set; }
    }
}