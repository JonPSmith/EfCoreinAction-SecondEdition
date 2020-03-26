// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;

namespace ServiceLayer.AdminServices.Concrete
{
    public class ChangePubDateService : IChangePubDateService
    {
        private readonly EfCoreContext _context;

        public ChangePubDateService(EfCoreContext context)
        {
            _context = context;
        }

        public ChangePubDateDto GetOriginal(int id)    //#A
        {
            return _context.Books
                .Select(p => new ChangePubDateDto      //#B
                {                                      //#B
                    BookId = p.BookId,                 //#B
                    Title = p.Title,                   //#B
                    PublishedOn = p.PublishedOn        //#B
                })                                     //#B
                .Single(k => k.BookId == id);          //#C
        }

        public Book UpdateBook(ChangePubDateDto dto)   //#D
        {
            var book = _context.Find<Book>(dto.BookId);//#E
            book.PublishedOn = dto.PublishedOn;        //#F
            _context.SaveChanges();                    //#G
            return book;                               //#H
        }
    }
    /*********************************************************
    #A This method handles the first part of the update, i.e. by getting the data from the chosen book to show to the user
    #B This is a select load query, which only returns three properties
    #C This uses the primary key to select the exact row we want to update
    #D This method handles the second part of the update, i.e. performing a selective update of the chosen book
    #E This loads the book. The EF Core Find method is an efficient way of loading a row by its primary key
    #F This is the selective update of just the PublishedOn property of the loaded book
    #G The SaveChanges uses its DetectChanges method to find out what has changed, and then updates the database
    #H The method returns the updated book
     * ******************************************************/
}