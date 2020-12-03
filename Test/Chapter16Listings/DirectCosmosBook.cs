// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using BookApp.Domain.Books;
using Newtonsoft.Json;

namespace Test.Chapter16Listings
{
    public class DirectCosmosBook : CosmosBook
    {
        private DirectCosmosBook() : base() {}

        public string Discriminator { get; set; } = "CosmosBook";

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public DirectCosmosBook(int bookId, Book book)
        {
            if (book == null)
                return;

            BookId = bookId;
            Id = $"{Discriminator}|{bookId}";

            Title = book.Title;
            PublishedOn = book.PublishedOn;
            EstimatedDate = book.EstimatedDate;
            OrgPrice = book.OrgPrice;
            ActualPrice = book.ActualPrice;
        }
    }

}