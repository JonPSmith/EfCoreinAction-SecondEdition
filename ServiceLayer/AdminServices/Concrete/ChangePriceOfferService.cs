// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
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

            return OrgBook.Promotion               //#C
                ?? new PriceOffer                  //#C
                   {                               //#C
                       BookId = id,                //#C
                       NewPrice = OrgBook.Price    //#C
                   };                              //#C
        }
        /*********************************************************
        #A This method gets a PriceOffer class to send to the user to update
        #B This loads the book with any existing Promotion
        #C I return either the existing Promotion for editing, or create a new one. The important point is to set the BookId, as we need to pass that through to the second stage
         * ******************************************************/

        /// <summary>
        /// This deletes a promotion if the book has one, otherwise is adds a promotion
        /// </summary>
        /// <param name="promotion"></param>
        /// <returns></returns>
        public ValidationResult AddRemoveBook(PriceOffer promotion)//#A
        {
            var book = _context.Books                              //#B
                .Include(r => r.Promotion)                         //#B
                .Single(k => k.BookId                              //#B
                             == promotion.BookId);                 //#B

            if (book.Promotion != null)                            //#C
            {
                _context.Remove(promotion.PromotionalText);        //#D
                _context.SaveChanges();                            //#D
                return null; //#E
            }

            if (string.IsNullOrEmpty(promotion.PromotionalText))   //#F
            {
                return new ValidationResult(                       //#G
                    "This field cannot be empty",                  //#G
                    new []{ nameof(PriceOffer.PromotionalText)});  //#G
            }

            book.Promotion = promotion;                            //#H
            _context.SaveChanges();                                //#H

            return null;                                           //#I
        }
        /*********************************************************
        #A This method deletes a promotion if the book has one, otherwise is adds a promotion. It returns null if it was successful, or a ValidationResult class if there was an error
        #B This loads the book, with any existing promotion
        #C If there is an existing Promotion on the book, then we need to remove that promotion
        #D This deletes the PriceOffer entry that was linked to the chosen book
        #E It returns null, which means the method finished successfully 
        #F We are going to add a promotion, and one of the business rules is that the PromotionalText must contains some text
        #G This returns an error message, with the particular property name that was incorrect. This allows the ASP.NET Core show an error next to the input that has an error
        #H This assigns the new PriceOffer. which was filled in by the user, to the selected book. Then it calls SaveChanges to update the database
        #I The adding of a new price promotion was successful so the method returns null
         * ******************************************************/
    }
}
