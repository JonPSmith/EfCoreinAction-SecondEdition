// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;

namespace ServiceLayer.AdminServices.Concrete
{
    public class ChangePubDateService : IChangePubDateService //#A
    {
        private readonly EfCoreContext _context;           //#B
                                                           //#B
        public ChangePubDateService(EfCoreContext context) //#B
        {                                                  //#B
            _context = context;                            //#B
        }                                                  //#B

        public ChangePubDateDto GetOriginal(int id)        //#C
        {
            return _context.Books
                .Select(p => new ChangePubDateDto          //#D
                {                                          //#D
                    BookId = p.BookId,                     //#D
                    Title = p.Title,                       //#D
                    PublishedOn = p.PublishedOn            //#D
                })                                         //#D
                .Single(k => k.BookId == id);              //#E
        }

        public Book UpdateBook(ChangePubDateDto dto)       //#F
        {
            var book = _context.Books.SingleOrDefault(     //#G
                x => x.BookId == dto.BookId);              //#G
            if (book == null)                              //#H
                throw new ArgumentException(               //#H
                    "Book not found");                     //#H
            book.PublishedOn = dto.PublishedOn;            //#I
            _context.SaveChanges();                        //#J
            return book;                                   //#K
        }
    }
    /*********************************************************
    #A This interface is needed when registering this class in DI. You use DI in chapter 5 when building the ASP.NET Core BookApp
    #B The application's DbContext is provided via a class constructor. This it the normal way of building classes that you will use as a service in ASP.NET Core
    #C This method handles the first part of the update, i.e. by getting the data from the chosen book to show to the user
    #D This is a select load query, which only returns three properties
    #E This uses the primary key to select the exact row we want to update
    #F This method handles the second part of the update, i.e. performing a selective update of the chosen book
    #G This loads the book. I use SingleOrDefault because its slightly quicker than the Find method
    #H I catch the case where a book wasn't found and throw an exception
    #I This is the selective update of just the PublishedOn property of the loaded book
    #J The SaveChanges uses its DetectChanges method to find out what has changed, and then updates the database
    #K The method returns the updated book
     * ******************************************************/
}