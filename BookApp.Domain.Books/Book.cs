// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BookApp.Domain.Books.DomainEvents;
using BookApp.Domain.Books.SupportTypes;
using GenericEventRunner.DomainParts;
using StatusGeneric;

namespace BookApp.Domain.Books
{
    public class Book : EntityEventsBase, ISoftDelete
    {
        public const int PromotionalTextLength = 200;

        //-----------------------------------------------
        //relationships backing fields

        //Use uninitialized backing fields - this means we can detect if the collection was loaded
        private HashSet<Review> _reviews;
        private HashSet<BookAuthor> _authorsLink;
        private HashSet<Tag> _tags;

        //-----------------------------------------------
        //ctors/static factory

        private Book() { }  //#A //Needed by EF Core

        public static IStatusGeneric<Book> CreateBook(      //#B
            string title, DateTime publishedOn,              //#C
            bool estimatedDate,                              //#C
            string publisher, decimal price, string imageUrl,//#C
            ICollection<Author> authors,                     //#C
            ICollection<Tag> tags = null)                    //#D
        {
            var status = new StatusGenericHandler<Book>();  //#E
            if (string.IsNullOrWhiteSpace(title))         //#F
                status.AddError(                          //#F
                    "The book title cannot be empty.");   //#F

            var book = new Book                         //#G
            {                                           //#G
                Title = title,                          //#G
                PublishedOn = publishedOn,              //#G
                EstimatedDate = estimatedDate,          //#G
                Publisher = publisher,
                OrgPrice = price,
                ActualPrice = price,
                ImageUrl = imageUrl,
                //We need to initialise the AuthorsOrdered string when the entry is created
                //NOTE: We must NOT initialise the ReviewsCount and the ReviewsAverageVotes as they default to zero
                AuthorsOrdered = string.Join(", ", authors.Select(x => x.Name)),

                _tags = tags != null           //#H
                    ? new HashSet<Tag>(tags)   //#H
                    : new HashSet<Tag>(),      //#H
                _reviews = new HashSet<Review>()       //We add an empty list on create. I allows reviews to be added when building test data
            };
            if (authors == null)                                   //#I
                throw new ArgumentNullException(nameof(authors));  //#I

            byte order = 0;                               //#J
            book._authorsLink = new HashSet<BookAuthor>(  //#J
                authors.Select(a =>                       //#J
                    new BookAuthor(book, a, order++)));   //#J
            if (!book._authorsLink.Any())                             //#K
                status.AddError(                                      //#K
                    "You must have at least one Author for a book."); //#K

            return status.SetResult(book); //#L
        }
        /***********************************************************************************
        #A Creating a private constructor means people can't create the entity via a constructor
        #B The static CreateBook method returns a status with a valid Book (if there are no errors)
        #C These all the parameters that are needed to create a valid book
        #D The Tags are optional
        #E This creates a status that can return a result - in this case a Book
        #F This adds an error. Note it doesn't return immediately so that other errors can be added
        #G Now you set the properties 
        #H This sets up the Tags collection via the backing field
        #I The authors parameter being null is considered a software error and throws an exception
        #J This creates the BookAuthor class in the order that the Authors have been provided
        #K If there are no Authors we add an error
        #L This sets the status's Result to the new Book instance. But if there are errors it will be null
         ************************************************************************/

        //---------------------------------------
        //scalar properties

        public int BookId { get; private set; }

        [Required(AllowEmptyStrings = false)]
        public string Title { get; private set; }

        public DateTime PublishedOn { get; private set; }
        public bool EstimatedDate { get; private set; }

        public string Publisher { get; private set; }
        public decimal OrgPrice { get; private set; }
        public decimal ActualPrice { get; private set; }

        [MaxLength(PromotionalTextLength)]
        public string PromotionalText { get; private set; }

        [MaxLength(200)]
        public string ImageUrl { get; private set; }

        /// <summary>
        /// This contains the url to get to the Manning version of the book
        /// </summary>
        public string ManningBookUrl { get; private set; }

        //---------------------------------------------
        //Soft delete

        public bool SoftDeleted { get; private set; }

        //---------------------------------------
        //relationships

        public IReadOnlyCollection<Review> Reviews => _reviews?.ToList();
        public IReadOnlyCollection<BookAuthor> AuthorsLink => _authorsLink?.ToList();
        public IReadOnlyCollection<Tag> Tags => _tags?.ToList();

        //----------------------------------------------
        //Table splitting

        public BookDetails Details { get; private set; }

        //----------------------------------------------
        //Extra properties filled in by events

        [ConcurrencyCheck]
        public string AuthorsOrdered { get; set; }

        [ConcurrencyCheck]
        public int ReviewsCount { get; private set; }

        [ConcurrencyCheck]
        public double ReviewsAverageVotes { get; private set; }

        //This is an action provided in the review add/remove event so that the review handler can update these properties
        private void UpdateReviewCachedValues(int reviewsCount, double reviewsAverageVotes)
        {
            ReviewsCount = reviewsCount;
            ReviewsAverageVotes = reviewsAverageVotes;
        }
        //----------------------------------------------

        public override string ToString()
        {
            var authors = _authorsLink?.OrderBy(x => x.Order).Select(x => x.Author.Name);
            var authorString = authors == null
                ? "(Cached) " + AuthorsOrdered
                : string.Join(", ", authors);
            var reviewsString = _reviews == null
                ? $"(Cached) {ReviewsCount} reviews"
                : $"{_reviews.Count()} reviews";
            //var tagsString = _bookTags == null
            //    ? ""
            //    : $" Tags: " + string.Join(", ", _tags.Select(x => x.TagId));

            return $"{Title}: by {authorString}. Price {ActualPrice}, {reviewsString}. Published {PublishedOn:d}";
        }

        //-----------------------------------------------------
        //DDD methods

        public void SetBookDetails(string description, string aboutAuthor, string aboutReader, 
            string aboutTechnology, string whatsInside)
        {
            if (Details == null)
                Details = new BookDetails();
            Details.SetBookDetails(description, aboutAuthor, aboutReader, aboutTechnology, whatsInside, this);
        }

        public void SetManningBookUrl(string manningBookUrl)
        {
            ManningBookUrl = manningBookUrl;
        }

        public void AlterSoftDelete(bool softDeleted)
        {
            SoftDeleted = softDeleted;
        }

        public void UpdatePublishedOnDay(DateTime publishedOn)
        {
            PublishedOn = publishedOn;
        }

        //This works with the GenericServices' IncludeThen Attribute to pre-load the Reviews collection
        public void AddReview(int numStars,   //#A
            string comment, string voterName) //#A
        {
            if (_reviews == null)                      //#B
                throw new InvalidOperationException(   //#B
                    "The Reviews collection must be loaded before calling this method");
            _reviews.Add(new Review(           //#C
                numStars, comment, voterName));    //#C

            AddEvent(new BookReviewAddedEvent(numStars, this, UpdateReviewCachedValues));
        }

        //This works with the GenericServices' IncludeThen Attribute to pre-load the Reviews collection
        public void RemoveReview(int reviewId) //#D
        {
            if (_reviews == null)                    //#B
                throw new InvalidOperationException( //#B
                    "The Reviews collection must be loaded before calling this method");
            var localReview = _reviews.SingleOrDefault(  //#E
                x => x.ReviewId == reviewId);            //#E
            if (localReview == null)                  //#F
                throw new InvalidOperationException(  //#F
                    "The review with that key was not found in the book's Reviews.");
            _reviews.Remove(localReview);             //#G
            
            AddEvent(new BookReviewRemovedEvent(localReview, this, UpdateReviewCachedValues));
        }
        /*****************************************************************
        #A This adds a new review with the given parameters
        #B This code relies on the _reviews being loaded so it throws an exception if it isn't
        #C This creates a new Review using its internal constructor
        #D This removes a review using its primary key
        #E This finds the specific Review to remove
        #F Not finding the review is considered a software error, so it throws an exception
        #G The found review is removed
         ******************************************************************/

        public IStatusGeneric AddPromotion(               //#A
            decimal actualPrice, string promotionalText)  //#B               
        {
            var status = new StatusGenericHandler();      //#C
            if (string.IsNullOrWhiteSpace(promotionalText)) //#D
            {
                return status.AddError(                        //#E
                    "You must provide text to go with the promotion.", //#F
                    nameof(PromotionalText));     //#F
            }

            ActualPrice = actualPrice;         //#G
            PromotionalText = promotionalText; //#G

            return status; //#H
        }

        public void RemovePromotion() //#H
        {
            ActualPrice = OrgPrice; //#I
            PromotionalText = null; //#I
        }
        /******************************************************************
        #A The AddPromotion return a status: successful if no error and returns errors 
        #B The parameters came from the input
        #C This creates a status which is successful unless errors are added to it
        #D You ensure the promotionalText has some text in it
        #E The AddError method adds an error, and it returned immediately
        #F The error contains a user-friendly message and the name of the property that has the error
        #G If no errors, then the ActualPrice and PromotionalText are updated
        #H The status, which is successful, is returned
         *******************************************************************/
    }

}