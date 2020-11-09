// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Infrastructure.Books.CachedValues;
using BookApp.Infrastructure.Books.CachedValues.CheckFixCode;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books.Dtos;
using BookApp.UI.HelperExtensions;
using GenericServices;
using GenericServices.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace BookApp.UI.Controllers
{
    public class AdminController : BaseTraceController
    {
        public async Task<IActionResult> ChangePubDate(int id, [FromServices]ICrudServicesAsync<BookDbContext> service) 
        {
            Request.ThrowErrorIfNotLocal(); 
            var dto = await service.ReadSingleAsync<ChangePubDateDto>(id); 
            SetupTraceInfo();
            return View(dto); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePubDate(ChangePubDateDto dto, [FromServices] ICrudServicesAsync<BookDbContext> service)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            await service.UpdateAndSaveAsync(dto);
            SetupTraceInfo();
            if (service.IsValid)
                return View("BookUpdated", service.Message);

            //Error state
            service.CopyErrorsToModelState(ModelState, dto);
            return View(dto);
        }

        public async Task<IActionResult> AddPromotion(int id, [FromServices] ICrudServicesAsync<BookDbContext> service)
        {
            Request.ThrowErrorIfNotLocal();
            var dto = await service.ReadSingleAsync<AddPromotionDto>(id);
            SetupTraceInfo();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPromotion(AddPromotionDto dto, [FromServices] ICrudServicesAsync<BookDbContext> service)
        {
            Request.ThrowErrorIfNotLocal();
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            await service.UpdateAndSaveAsync(dto);
            SetupTraceInfo();
            if (!service.HasErrors)
                return View("BookUpdated", service.Message);

            //Error state
            service.CopyErrorsToModelState(ModelState, dto);
            return View(dto);
        }

        public async Task<IActionResult> RemovePromotion(int id, [FromServices] ICrudServicesAsync<BookDbContext> service)
        {
            var dto = await service.ReadSingleAsync<RemovePromotionDto>(id);
            if (!service.IsValid)
            {
                service.CopyErrorsToModelState(ModelState, dto);
            }
            SetupTraceInfo();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePromotion(RemovePromotionDto dto, [FromServices] ICrudServicesAsync<BookDbContext> service)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            await service.UpdateAndSaveAsync(dto, nameof(Book.RemovePromotion));
            SetupTraceInfo();
            if (service.IsValid)
                return View("BookUpdated", service.Message);

            //Error state
            service.CopyErrorsToModelState(ModelState, dto);
            return View(dto);
        }


        public async Task<IActionResult> AddBookReview(int id, [FromServices] ICrudServicesAsync<BookDbContext> service)
        {
            var dto = await service.ReadSingleAsync<AddReviewDto>(id);
            if (!service.IsValid)
            {
                service.CopyErrorsToModelState(ModelState, dto);
            }
            SetupTraceInfo();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBookReview(AddReviewDto dto, [FromServices] ICrudServicesAsync<BookDbContext> service)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            await service.UpdateAndSaveAsync(dto);
            SetupTraceInfo();
            if (service.IsValid)
                return View("BookUpdated", service.Message);

            //Error state
            service.CopyErrorsToModelState(ModelState, dto);
            return View(dto);
        }


        public IActionResult CacheCheckFix()
        {
            Request.ThrowErrorIfNotLocal();
            SetupTraceInfo();
            return View(new CheckFixInputDto{ FixBadCacheValues = true, LookingBack = new TimeSpan(0,1,00,0)});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CacheCheckFix(CheckFixInputDto dto, CancellationToken cancellationToken, 
            [FromServices]ICheckFixCacheValuesService service)
        {
            Request.ThrowErrorIfNotLocal();
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var scanFrom = DateTime.UtcNow.Subtract(dto.LookingBack);
            await service.RunCheckAsync(scanFrom, dto.FixBadCacheValues, cancellationToken );
            SetupTraceInfo();
            return View("BookUpdated", "The CheckFixCache service has finished - see logs for info.");
        }

    }
}