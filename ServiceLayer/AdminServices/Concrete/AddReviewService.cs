// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

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
}
