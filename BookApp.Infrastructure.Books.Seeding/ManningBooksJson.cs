// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using BookApp.Domain.Books;

namespace BookApp.Infrastructure.Books.Seeding
{
    public class ManningBooksJson
    {
        public DateTime expectedPublishDate { get; set; }
        public bool hasAudio { get; set; }
        public string authorshipDisplay { get; set; }
        public string isbn { get; set; }
        public string externalId { get; set; }
        public DateTime lastSignificantDate { get; set; }
        public string title { get; set; }
        public string seoKeywords { get; set; }
        public string[] tags { get; set; }
        public bool livebook { get; set; }
        public Productoffering[] productOfferings { get; set; }
        public string subtitle { get; set; }
        public string imageUrl { get; set; }
        public bool isLiveVideo { get; set; }
        public bool isLiveProject { get; set; }
        public int id { get; set; }
        public DateTime? publishedDate { get; set; }
        public string slug { get; set; }

        public Book MapToBook()
        {
            var fullImageUrl = "https://images.manning.com/360/480/resize/" + imageUrl;
            var publishedOn = publishedDate ?? expectedPublishDate;
            var price = productOfferings.Select(x => x.price).Max();


            throw new NotImplementedException();
        }
    }

    public class Productoffering
    {
        public float price { get; set; }
        public int id { get; set; }
        public string productType { get; set; }
    }

}