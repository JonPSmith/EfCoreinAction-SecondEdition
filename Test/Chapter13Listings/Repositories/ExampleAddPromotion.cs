// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books.Dtos;
using GenericServices;
using GenericServices.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Test.Chapter13Listings.EfClasses;

namespace Test.Chapter13Listings.Repositories
{
    public class ExampleAddPromotion
    //public class AdminController : Controller
    {
        private readonly GenericRepository<Book> _repository; //#A

        public ExampleAddPromotion(             //#A
            GenericRepository<Book> repository) //#A
        {                                       //#A
            _repository = repository;           //#A
        }                                       //#A

        //public async Task<IActionResult> AddPromotion(int id)
        public async Task<AddPromotionDto> AddPromotion(int id)
        {
            var book = await _repository.FindEntityAsync(id); //#B
            var dto = new AddPromotionDto //#C
            {                             //#C
                BookId = id,              //#C
                Title = book.Title,       //#C
                OrgPrice = book.OrgPrice  //#C
            };                            //#C
            //return View(dto);
            return dto;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddPromotion(AddPromotionDto dto)
        public async Task AddPromotion(AddPromotionDto dto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(dto);
            //}
            var book = await _repository      //#B
                .FindEntityAsync(dto.BookId); //#B
            var status = book.AddPromotion(            //#D
                dto.ActualPrice, dto.PromotionalText); //#D

            if (!status.HasErrors)
            {
                await _repository.PersistDataAsync(); //#E
                //return View("BookUpdated", service.Message);
            }

            //Error state
            //service.CopyErrorsToModelState(ModelState, dto);
            //return View(dto);
        }
        /****************************************************************
        #A The GenericRepository<Book> is injected into the Controller 
        #B Use the repository to read in the Book entity
        #C Copy over the parts of the Book you need to show the page
        #D You call the AddPromotion access method with the two properties from the dto
        #E The access method returned no errors, so you persist the data to the database
         ****************************************************************/

    }
}