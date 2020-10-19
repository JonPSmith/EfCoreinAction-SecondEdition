// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading.Tasks;
using BookApp.ServiceLayer.DefaultSql.Books.Dtos;
using GenericServices;
using Microsoft.AspNetCore.Mvc;

namespace Test.Chapter13Listings.Examples
{
    public class GenericServicesAddPromotion
    //public class AdminController : Controller
    {
        private readonly ICrudServicesAsync _service; //#A

        public GenericServicesAddPromotion(             //#A
            ICrudServicesAsync service) //#A
        {                                       //#A
            _service = service;           //#A
        }                                       //#A

        //public async Task<IActionResult> AddPromotion(int id)
        public async Task<AddPromotionDto> AddPromotion(int id)
        {
            var dto = await _service    //#B
                .ReadSingleAsync<AddPromotionDto>(id); //#B
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

            await _service.UpdateAndSaveAsync(dto); //#C

            if (!_service.HasErrors)
            {
                //return View("BookUpdated", service.Message);
            }

            //Error state
            //_service.CopyErrorsToModelState(ModelState, dto);
            //return View(dto);
        }
    }
    /****************************************************************
    #A The ICrudServicesAsync service is injected via the Controller's constructor
    #B The ReadSingleAsync<T> will read into the DTO using the given primary key
    #C The UpdateAndSaveAsync method calls the access method, and if no errors it saves it to the database
     ****************************************************************/
}