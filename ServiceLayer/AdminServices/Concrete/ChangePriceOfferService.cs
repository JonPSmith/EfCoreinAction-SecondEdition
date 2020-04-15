// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.Linq;
using BizLogic.GenericInterfaces;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;

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

        public void UpdateBook(PriceOffer promotion)//#D
        {
            var book = _context.Books               //#E
                .Include(r => r.Promotion)        //#E
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
         * ******************************************************/

        public IStatusGeneric AddPromotionWithChecks //#A
            (PriceOffer promotion) //#A
        {
            var status = new StatusGenericHandler(); //#B
            if (string.IsNullOrEmpty( promotion.PromotionalText)) //#C
            {
                status.AddError("This field cannot be empty", //#D
                    nameof(PriceOffer.PromotionalText));                 //#D
                return status;                                           //#D
            }

            //... rest of the CRUD code left out
            var book = _context.Books 
                .Include(r => r.Promotion) 
                .Single(k => k.BookId 
                             == promotion.BookId);
            if (book.Promotion == null)             
            {
                book.Promotion = promotion;         
            }
            else
            {
                book.Promotion.NewPrice             
                    = promotion.NewPrice;           
                book.Promotion.PromotionalText      
                    = promotion.PromotionalText;    
            }
            _context.SaveChanges();  //#E               

            return status; //#F
        }
        /*********************************************************
        #A This method returns the IStatusGeneric, which has an IsValid property of true if there are no errors
        #B This creates a status handler which has an Errors list that you can appended to via the AddError method
        #C The error is added, with the name of the property that failed, so that it can be added to the ASP.NET Core's ModelState to show on the web page 
        #D This method handles the second part of the update, i.e. performing a selective update of the chosen book
        #E The business logic calls SaveChanges because this service is self-contained
        #F If the promotion was added successfully the status would return with the IsValid property set to true 
         * ******************************************************/
    }
}
