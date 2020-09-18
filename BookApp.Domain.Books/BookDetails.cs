// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace BookApp.Domain.Books
{
    public class BookDetails
    {
        public int BookDetailsId { get; private set; }

        public string Description { get; private set; }
        public string AboutAuthor { get; private set; }
        public string AboutReader { get; private set; }
        public string AboutTechnology { get; private set; }
        public string WhatsInside { get; private set; }

        //----------------------------------------------
        //Any concurrency tokens must appear in all classes mapped to the table

        [ConcurrencyCheck]
        public string AuthorsOrdered { get; set; }

        [ConcurrencyCheck]
        public int ReviewsCount { get; private set; }

        [ConcurrencyCheck]
        public double ReviewsAverageVotes { get; private set; }

        internal void SetBookDetails(string description, string aboutAuthor, string aboutReader,
            string aboutTechnology, string whatsInside, Book book)
        {
            Description = description;
            AboutAuthor = aboutAuthor;
            AboutReader = aboutReader;
            AboutTechnology = aboutTechnology;
            WhatsInside = whatsInside;

            AuthorsOrdered = book.AuthorsOrdered;
            ReviewsCount = book.ReviewsCount;
            ReviewsAverageVotes = book.ReviewsAverageVotes;
        }
    }
}