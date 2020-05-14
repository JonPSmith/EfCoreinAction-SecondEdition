// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;

namespace ServiceLayer.AdminServices.Concrete
{
    public class AddReviewService : IAddReviewService
    {
        private readonly EfCoreContext _context;

        public AddReviewService(EfCoreContext context)
        {
            _context = context;
        }

        public string BookTitle { get; private set; }

        public Review GetBlankReview(int id) //#A
        {
            BookTitle = _context.Books     //#B
                .Where(p => p.BookId == id)//#B
                .Select(p => p.Title)      //#B
                .Single();                 //#B
            return new Review//#C
            {                //#C
                BookId = id  //#C
            };               //#C
        }

        public Book AddReviewToBook(Review review)//#D
        {
            var book = _context.Books   //#E
                .Include(r => r.Reviews)//#E
                .Single(k => k.BookId   //#E
                      == review.BookId);//#E
            book.Reviews.Add(review); //#F
            _context.SaveChanges(); //#G
            return book; //#H
        }

    /*********************************************************
    #A This method forms a Review to be filled in by the user
    #B I read the book title to show to the user when they are filling in their review
    #C This simply creates a Review with the BookId foreign key filled in
    #D Ths method updates the book with the new review
    #E This loads the correct book using the value in the review's foreign key, and includes any existing reviews (or empty collection if no reviews yet)
    #F I now add the new review to the Reviews collection
    #G The SaveChanges uses its DetectChanges method, which sees that the Book Review property has changed. It then creates a new row in the Review table
    #H The method returns the updated book
     * ******************************************************/

        public IStatusGeneric AddReviewWithChecks(Review review)              //#A
        {
            var status = new StatusGenericHandler();                          //#B
            if (review.NumStars < 0 || review.NumStars > 5)                   //#C
                status.AddError("This must be between 0 and 5.",              //#C
                    nameof(Review.NumStars));                                 //#C
            if (string.IsNullOrWhiteSpace(review.Comment))                    //#D
                status.AddError("Please provide a comment with your review.", //#D
                    nameof(Review.Comment));                                  //#D
            if (!status.IsValid)                                             //#E
                return status;                                                //#E

            var book = _context.Books                                         //#F
                .Include(r => r.Reviews)                                      //#F
                .Single(k => k.BookId                                         //#F
                             == review.BookId);                               //#F
            book.Reviews.Add(review);                                         //#F
            _context.SaveChanges();                                           //#F
            return status;                                                    //#G
        }
        //0123456789|123456789|123456789|123456789|123456789|123456789|123456789|xxxxx!
        /***********************************************************
        #A This method adds a review to a book, with validation checks on the data
        #B It creates a status class to hold any errors
        #C This adds an error to the status if the star rating is in the correct range
        #D This second check ensures the user has provided some sort of comment
        #E If there are any errors the method returns immediately with those errors.
        #F This is the CRUD code that adds a review to a book
        #G This returns the status, which will be valid if no errors were found
         **************************************************************/
    }
}
