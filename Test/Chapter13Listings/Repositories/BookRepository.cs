// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.


using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Test.Chapter13Listings.EfClasses;

namespace Test.Chapter13Listings.Repositories
{
    public class BookRepository : GenericRepository<Book> //#A
    {
        public BookRepository(DbContext context) //#B
            : base(context)                     //#B
        { }

        public async Task<Book> 
            LoadBookWithReviewsAsync(int bookId) //#C
        {
            var book = await GetEntities()       //#D
                .Include(b => b.Reviews)  //#E
                .SingleOrDefaultAsync(    //#F
                    b => b.BookId == bookId); //#F
            if (book == null)                                //#G
                throw new Exception("Could not find book");  //#G
            return book; //#H
        }

    }
    /**************************************************************
    #A The book repository inherits the generic repository to get the general commands
    #B The GenericRepository needs the application's DbContext
    #C This loads a Book with Reviews
    #D You use the GenericRepository's GetEntities to get a IQueryable<Book> query
    #E This makes sure the Review collection is loaded with the book
    #F And you select the Book with the given BookId
    #G This a rudimentary check that the entity was found
    #H Finally you return the book with the Reviews collection loaded
     ***************************************************************/
}