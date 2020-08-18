// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Globalization;
using System.Linq;
using BookApp.UI.HelperExtensions;
using DataLayer.EfClasses;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.AdminServices;

namespace BookApp.UI.Controllers
{
    public class AdminController : BaseTraceController
    {
        public IActionResult ChangePubDate(int id, [FromServices] IChangePubDateService service) 
        {
            Request.ThrowErrorIfNotLocal(); 
            var dto = service.GetOriginal(id); 
            SetupTraceInfo();
            return View(dto); 
        }


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