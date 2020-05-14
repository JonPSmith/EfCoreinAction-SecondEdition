// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Globalization;
using System.Linq;
using BookApp.HelperExtensions;
using DataLayer.EfClasses;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.AdminServices;

namespace BookApp.Controllers
{
    public class AdminController : BaseTraceController
    {


        public IActionResult ChangePubDate //#A
        (int id, //#B
            [FromServices] IChangePubDateService service) //#C
        {
            Request.ThrowErrorIfNotLocal(); //REMOVE THIS FOR BOOK as it isn't relevant
            var dto = service.GetOriginal(id); //#D
            SetupTraceInfo(); //REMOVE THIS FOR BOOK as it could be confusing
            return View(dto); //#E
        }
        /**************************************************
        #A This is the action that is called if the user clicks the Admin->Change Pub Date link
        #B It receives the primary key of the book that the user wants to change
        #C This is where ASP.NET DI injects the ChangePubDateService instance
        #D Now we use the service to set up a dto to show the user
        #E This shows the user the page that allows them to edit the publication date
         * ************************************************/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePubDate(ChangePubDateDto dto,
            [FromServices] IChangePubDateService service)
        {
            Request.ThrowErrorIfNotLocal();
            service.UpdateBook(dto);
            SetupTraceInfo(); //REMOVE THIS FOR BOOK as it could be confusing
            return View("BookUpdated", "Successfully changed publication date");
        }

        public IActionResult ChangePromotion(int id, [FromServices] IChangePriceOfferService service)
        {
            Request.ThrowErrorIfNotLocal();

            var priceOffer = service.GetOriginal(id);
            ViewData["BookTitle"] = service.OrgBook.Title;
            ViewData["OrgPrice"] = service.OrgBook.Price < 0
                ? "Not currently for sale"
                : service.OrgBook.Price.ToString("c", new CultureInfo("en-US"));
            SetupTraceInfo();
            return View(priceOffer);
        }

        [HttpPost]                                                 
        [ValidateAntiForgeryToken]                                 
        public IActionResult ChangePromotion(PriceOffer dto,       
            [FromServices] IChangePriceOfferService service)       
        {
            Request.ThrowErrorIfNotLocal();

            var error =  service.AddRemovePriceOffer(dto);               
            if (error != null)                                     
            {
                ModelState.AddModelError(error.MemberNames.First(), 
                    error.ErrorMessage);                           
                return View(dto);                                  
            }
            SetupTraceInfo();
            return View("BookUpdated",                             
                "Successfully added/changed a promotion");         
        }

        public IActionResult AddBookReview(int id, [FromServices] IAddReviewService service)
        {
            Request.ThrowErrorIfNotLocal();

            var review = service.GetBlankReview(id);
            ViewData["BookTitle"] = service.BookTitle;
            SetupTraceInfo();
            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBookReview(Review dto, [FromServices] IAddReviewService service)
        {
            Request.ThrowErrorIfNotLocal();

            var book = service.AddReviewToBook(dto);
            SetupTraceInfo();
            return View("BookUpdated", "Successfully added a review");
        }
    }
}