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
    public class Book : EventsAndCreatedUpdated, ISoftDelete
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

        private Book() { }   //Needed by EF Core

        public static IStatusGeneric<Book> CreateBook(      
            string title, DateTime publishedOn,              
            bool estimatedDate,                              
            string publisher, decimal price, string imageUrl,
            ICollection<Author> authors,                     
            ICollection<Tag> tags = null)                    
        {
            var status = new StatusGenericHandler<Book>();  
            if (string.IsNullOrWhiteSpace(title))         
                status.AddError(                          
                    "The book title cannot be empty.");   

            var book = new Book                         
            {                                           
                Title = title,                          
                PublishedOn = publishedOn,              
                EstimatedDate = estimatedDate,          
                Publisher = publisher,
                OrgPrice = price,
                ActualPrice = price,
                ImageUrl = imageUrl,
                //We need to initialise the AuthorsOrdered string when the entry is created
                //NOTE: We must NOT initialise the ReviewsCount and the ReviewsAverageVotes as they default to zero
                AuthorsOrdered = string.Join(", ", authors.Select(x => x.Name)),

                _tags = tags != null           
                    ? new HashSet<Tag>(tags)   
                    : new HashSet<Tag>(),      
                _reviews = new HashSet<Review>()       //We add an empty list on create. I allows reviews to be added when building test data
            };
            if (authors == null)                                   
                throw new ArgumentNullException(nameof(authors));  

            byte order = 0;                               
            book._authorsLink = new HashSet<BookAuthor>(  
                authors.Select(a =>                       
                    new BookAuthor(book, a, order++)));   
            if (!book._authorsLink.Any())                             
                status.AddError(                                      
                    "You must have at least one Author for a book."); 
            
            if (status.IsValid)
                book.AddEvent(new BookChangedEvent(BookChangeTypes.Added), EventToSend.DuringSave);

            return status.SetResult(book); 
        }

        /// <summary>
        /// This static factory is used by the BookGenerator when filling the database with test data
        /// This is designed to work without the GenericEventRunner running 
        /// </summary>
        public Book(string title, DateTime publishedOn,
            bool estimatedDate, string publisher,
            decimal price, string imageUrl,
            ICollection<Author> authors,
            ICollection<Tag> tags,
            ICollection<byte> reviewNumStars, string reviewVoterName)
        {
            if (authors == null) throw new ArgumentNullException(nameof(authors));
            if (tags == null) throw new ArgumentNullException(nameof(tags));

            Title = title ?? throw new ArgumentNullException(nameof(title));
            PublishedOn = publishedOn;
            EstimatedDate = estimatedDate;
            Publisher = publisher;
            OrgPrice = price;
            ActualPrice = price;
            ImageUrl = imageUrl;

            if (authors == null)
                throw new ArgumentNullException(nameof(authors));

            byte order = 0;
            _authorsLink = new HashSet<BookAuthor>(
                authors.Select(a =>
                    new BookAuthor(this, a, order++)));
            if (!_authorsLink.Any())
                throw new ArgumentNullException(nameof(authors));
            //Set AuthorsOrdered cached value
            AuthorsOrdered = string.Join(", ", authors.Select(x => x.Name));

            _tags = new HashSet<Tag>(tags);


            if (!reviewNumStars.Any()) return; //no reviews to add

            //Now we add the reviews for the generated book
            _reviews = new HashSet<Review>();
            foreach (var numStar in reviewNumStars)
            {
                _reviews.Add(new Review(numStar, null, reviewVoterName));
            }
            //And set up the reviews cached values
            ReviewsCount = _reviews.Count;
            ReviewsAverageVotes = _reviews.Average(y => y.NumStars);
        }

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
        public string AuthorsOrdered { get; private set; }

        public void ResetAuthorsOrdered(string authorOrdered)
        {
            AuthorsOrdered = authorOrdered;
        }

        [ConcurrencyCheck]
        public int ReviewsCount { get; private set; }

        [ConcurrencyCheck]
        public double ReviewsAverageVotes { get; private set; }

        public void UpdateReviewCachedValues
            (int reviewsCount, double reviewsAverageVotes)
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
            var tagsString = _tags == null || !_tags.Any()
                ? ""
                : $" Tags: " + string.Join(", ", _tags.Select(x => x.TagId));

            return $"{Title}: by {authorString}. Price {ActualPrice}, {reviewsString}. Published {PublishedOn:d}{tagsString}";
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
            AddEvent(new BookChangedEvent(BookChangeTypes.Updated), EventToSend.DuringSave);
        }

        public void AlterSoftDelete(bool softDeleted)
        {
            if (SoftDeleted != softDeleted)
            {
                var eventType = softDeleted
                    ? BookChangeTypes.Deleted
                    : BookChangeTypes.Added;

                AddEvent(new BookChangedEvent(eventType)
                    , EventToSend.DuringSave);
            }
            SoftDeleted = softDeleted;
        }

        public void UpdatePublishedOn(DateTime publishedOn)
        {
            PublishedOn = publishedOn;
            AddEvent(new BookChangedEvent(BookChangeTypes.Updated), EventToSend.DuringSave);
        }

        //This works with the GenericServices' IncludeThen Attribute to pre-load the Reviews collection
        public void AddReview(int numStars,   //#B
            string comment, string voterName) //#B
        {
            if (_reviews == null)                      
                throw new InvalidOperationException(   
                    "The Reviews collection must be loaded before calling this method");
            _reviews.Add(new Review(           
                numStars, comment, voterName));    

            AddEvent(new BookReviewAddedEvent(numStars, //#C
                UpdateReviewCachedValues)); //#D
            AddEvent(new BookChangedEvent(BookChangeTypes.Updated), EventToSend.DuringSave);
        }

        //This works with the GenericServices' IncludeThen Attribute to pre-load the Reviews collection
        public void RemoveReview(int reviewId) //#E
        {
            if (_reviews == null)                    
                throw new InvalidOperationException( 
                    "The Reviews collection must be loaded before calling this method");
            var localReview = _reviews.SingleOrDefault(  
                x => x.ReviewId == reviewId);            
            if (localReview == null)                  
                throw new InvalidOperationException(  
                    "The review with that key was not found in the book's Reviews.");
            _reviews.Remove(localReview);             
            
            AddEvent(new BookReviewRemovedEvent(localReview, //#F
                UpdateReviewCachedValues)); //#D
            AddEvent(new BookChangedEvent(BookChangeTypes.Updated), EventToSend.DuringSave);
        }

        /*******************************************************************
        #A Adding the EntityEventsBase will provide the methods to send an event
        #B The AddReview is the only way to add a Review to this Book
        #C This adds a BookReviewAddedEvent domain event with the NumStars of the new Review
        #D It provides the event handler with a secure way to update the Review cached values
        #E The RemoveReview method is the only way to remove a Review from this Book
        #F This add a BookReviewAddedEvent domain event with the review that has been deleted
        #G This private method can be used by the event handlers to update the cached values
         ********************************************************************/

        public IStatusGeneric AddPromotion(               
            decimal actualPrice, string promotionalText)                 
        {
            var status = new StatusGenericHandler();      
            if (string.IsNullOrWhiteSpace(promotionalText)) 
            {
                return status.AddError(                        
                    "You must provide text to go with the promotion.", 
                    nameof(PromotionalText));     
            }

            ActualPrice = actualPrice;         
            PromotionalText = promotionalText; 

            if (status.IsValid)
                AddEvent(
                    new BookChangedEvent(BookChangeTypes.Updated), 
                    EventToSend.DuringSave);

            return status; 
        }

        public void RemovePromotion() 
        {
            ActualPrice = OrgPrice; 
            PromotionalText = null;
            AddEvent(new BookChangedEvent(BookChangeTypes.Updated), EventToSend.DuringSave);
        }
    }

}