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
        //ctors / static create factory

        private Book() { } //#A

        public static IStatusGeneric<Book> CreateBook(      //#B
        string title, DateTime publishedOn,       //#C
        decimal price,                            //#C
        ICollection<Author> authors)              //#C
        {
            var status = new StatusGenericHandler<Book>();//#D
            if (string.IsNullOrWhiteSpace(title))         //#E
                status.AddError(                          //#E
                    "The book title cannot be empty.");   //#E

            var book = new Book                         //#F
            {                                           //#F
                Title = title,                          //#F
                PublishedOn = publishedOn,              //#F
                OrgPrice = price,                       //#F
                ActualPrice = price,                    //#F
            };
            if (authors == null)                                   //#G
                throw new ArgumentNullException(nameof(authors));  //#G

            byte order = 0;                               //#H
            book._authorsLink = new HashSet<BookAuthor>(  //#H
                authors.Select(a =>                       //#H
                    new BookAuthor(book, a, order++)));   //#H

            if (!book._authorsLink.Any())                             //#I
                status.AddError(                                      //#I
                    "You must have at least one Author for a book."); //#I

            return status.SetResult(book); //#J
        }
        /***********************************************************************************
        #A Creating a private constructor means people can't create the entity via a constructor
        #B The static CreateBook method returns a status with a valid Book (if there are no errors)
        #C These all the parameters that are needed to create a valid book
        #D This creates a status that can return a result - in this case a Book
        #E This adds an error. Note it doesn't return immediately so that other errors can be added
        #F Now you set the properties 
        #G This sets up the Tags collection via the backing field
        #H The authors parameter being null is considered a software error and throws an exception
        #I This creates the BookAuthor class in the order that the Authors have been provided
        #J If there are no Authors we add an error
        #K This sets the status's Result to the new Book instance. But if there are errors it will be null
         ************************************************************************/

        //----------------------------------------
        //properties

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

        //-----------------------------------------------------
        //DDD access methods

        public void UpdatePublishedOnDay(DateTime publishedOn)
        {
            PublishedOn = publishedOn;
        }

        public void AddReview(int numStars,   //#A
            string comment, string voterName) //#A
        {
            if (_reviews == null)                      //#B
                throw new InvalidOperationException(   //#B
                    "The Reviews collection must be loaded before calling this method");
            _reviews.Add(new Review(           //#C
                numStars, comment, voterName));    //#C
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