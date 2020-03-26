// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer.AdminServices.Concrete
{
    public class ChangePriceOfferService : IChangePriceOfferService
    {
        private readonly EfCoreContext _context;

        public ChangePriceOfferService(EfCoreContext context)
        {
            _context = context;
        }

        public Book OrgBook { get; private set; }

        public PriceOffer GetOriginal(int id)      //#A
        {
            OrgBook = _context.Books               //#B
                .Include(r => r.Promotion)         //#B
                .Single(k => k.BookId == id);      //#B

            return OrgBook?.Promotion              //#C
                ?? new PriceOffer                  //#C
                   {                               //#C
                       BookId = id,                //#C
                       NewPrice = OrgBook.Price    //#C
                   };                              //#C
        }

        public Book UpdateBook(PriceOffer promotion)//#D
        {
            var book = _context.Books               //#E
                .Include(r => r.Promotion)          //#E
                .Single(k => k.BookId               //#E
                      == promotion.BookId);         //#E
            if (book.Promotion == null)             //#F
            {
                book.Promotion = promotion;         //#G
            }
            else
            {
                book.Promotion.NewPrice             //#H
                    = promotion.NewPrice;           //#H
                book.Promotion.PromotionalText      //#H
                    = promotion.PromotionalText;    //#H
            }
            _context.SaveChanges();                 //#I
            return book;                            //#J
        }
    }
    /*********************************************************
    #A This method gets a PriceOffer class to send to the user to update
    #B This loads the book with any existing Promotion
    #C I return either the existing Promotion for editing, or create a new one. The important point is to set the BookId, as we need to pass that through to the second stage
    #D This method handles the second part of the update, i.e. performing a selective update of the chosen book
    #E This loads the book, with any existing promotion, which is important as otherwise our new PriceOffer will clash, and throw an error
    #F I check if this an update of an existing PriceOffer or adding a new PriceOffer
    #G I need to add a new ProceOffer, so I simply assign the promotion to the relational link. EF Core will see this and add a new row in the PriceOffer table
    #H I need to do an update, so I copy over just the parts that I want to change. EF Core will see this update and produce code to unpdate just these two columns
    #I The SaveChanges uses its DetectChanges method, which sees what has changes - either adding a new PriceOffer or updating an existing PriceOffer
    #H The method returns the updated book
     * ******************************************************/
}
