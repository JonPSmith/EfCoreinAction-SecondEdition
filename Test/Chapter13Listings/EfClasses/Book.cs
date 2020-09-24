// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using StatusGeneric;

namespace Test.Chapter13Listings.EfClasses
{
    public class Book
    {
        public const int PromotionalTextLength = 200;
        private HashSet<BookAuthor> _authorsLink;
        private HashSet<Review> _reviews;

        //-----------------------------------------------
        //ctors

        private Book() { } //Needed by EF Core

        public int BookId { get; private set; }
        [Required(AllowEmptyStrings = false)]
        public string Title { get; private set; }
        public DateTime PublishedOn { get; private set; }
        public decimal OrgPrice { get; private set; }
        public decimal ActualPrice { get; private set; }
        [MaxLength(PromotionalTextLength)]
        public string PromotionalText { get; private set; }


        //---------------------------------------
        //relationships

        public IReadOnlyCollection<Review> Reviews => _reviews?.ToList();
        public IReadOnlyCollection<BookAuthor> AuthorsLink => _authorsLink?.ToList();

        //----------------------------------------------

        public static IStatusGeneric<Book> CreateBook(
            string title, DateTime publishedOn, decimal price, 
            ICollection<Author> authors)
        {
            var status = new StatusGenericHandler<Book>();
            if (string.IsNullOrWhiteSpace(title))
                status.AddError("The book title cannot be empty.");

            var book = new Book
            {
                Title = title,
                PublishedOn = publishedOn,
                OrgPrice = price,
                ActualPrice = price,
                _reviews = new HashSet<Review>()       //We add an empty list on create. I allows reviews to be added when building test data
            };
            if (authors == null)
                throw new ArgumentNullException(nameof(authors));
            byte order = 0;
            book._authorsLink = new HashSet<BookAuthor>(authors.Select(a => new BookAuthor(book, a, order++)));
            if (!book._authorsLink.Any())
                status.AddError("You must have at least one Author for a book.");

            return status.SetResult(book);
        }


        //-----------------------------------------------------
        //DDD methods

        public void UpdatePublishedOnDay(DateTime publishedOn)
        {
            PublishedOn = publishedOn;
        }

        //This works with the GenericServices' IncludeThen Attribute to pre-load the Reviews collection
        public void AddReview(int numStars, string comment, string voterName)
        {
            if (_reviews == null)
                throw new InvalidOperationException("The Reviews collection must be loaded before calling this method");
            _reviews.Add(new Review(numStars, comment, voterName));
        }

        //This works with the GenericServices' IncludeThen Attribute to pre-load the Reviews collection
        public void RemoveReview(int reviewId)
        {
            if (_reviews == null)
                throw new InvalidOperationException("The Reviews collection must be loaded before calling this method");
            var localReview = _reviews.SingleOrDefault(x => x.ReviewId == reviewId);
            if (localReview == null)
                throw new InvalidOperationException("The review with that key was not found in the book's Reviews.");
            _reviews.Remove(localReview);
        }

        public IStatusGeneric AddPromotion(decimal actualPrice, string promotionalText)                  
        {
            var status = new StatusGenericHandler();
            if (string.IsNullOrWhiteSpace(promotionalText))
            {
                status.AddError("You must provide some text to go with the promotion.", nameof(PromotionalText));
                return status;
            }

            ActualPrice = actualPrice;  
            PromotionalText = promotionalText;

            status.Message = $"The book's new price is ${actualPrice:F}.";

            return status; 
        }

        public void RemovePromotion() 
        {
            ActualPrice = OrgPrice; 
            PromotionalText = null; 
        }
    }

}