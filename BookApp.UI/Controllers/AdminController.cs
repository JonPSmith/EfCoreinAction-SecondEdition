// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Infrastructure.AppParts;
using BookApp.Infrastructure.Books.CachedValues;
using BookApp.Infrastructure.Books.CachedValues.CheckFixCode;
using BookApp.Infrastructure.LoggingServices;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books.Dtos;
using BookApp.UI.HelperExtensions;
using BookApp.UI.Models;
using GenericServices;
using GenericServices.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookApp.UI.Controllers
{
    public class AdminController : BaseTraceController
    {
        private readonly string _backToDisplayController;

        public class BookUpdatedDto
        {
            public BookUpdatedDto(string message, string controllerName)
            {
                Message = message;
                ControllerName = controllerName;
            }

            public string Message { get; }
            public string ControllerName { get; } 
        }

        public AdminController(BookAppSettings settings)
        {
            _backToDisplayController = settings.GetDisplayControllerBasedOnTheMenuSet();
        }


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
                return View("BookUpdated", new BookUpdatedDto(service.Message, _backToDisplayController));

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
                return View("BookUpdated", new BookUpdatedDto(service.Message, _backToDisplayController));

            //Error state
            service.CopyErrorsToModelState(ModelState, dto);
            return View(dto);
        }

        public async Task<IActionResult> RemovePromotion(int id, [FromServices] ICrudServicesAsync<BookDbContext> service)
        {
            Request.ThrowErrorIfNotLocal();
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
            Request.ThrowErrorIfNotLocal();
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            await service.UpdateAndSaveAsync(dto, nameof(Book.RemovePromotion));
            SetupTraceInfo();
            if (service.IsValid)
                return View("BookUpdated", new BookUpdatedDto(service.Message, _backToDisplayController));

            //Error state
            service.CopyErrorsToModelState(ModelState, dto);
            return View(dto);
        }


        public async Task<IActionResult> AddBookReview(int id, [FromServices] ICrudServicesAsync<BookDbContext> service)
        {
            Request.ThrowErrorIfNotLocal();
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
            Request.ThrowErrorIfNotLocal();
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            await service.UpdateAndSaveAsync(dto);
            SetupTraceInfo();
            if (service.IsValid)
                return View("BookUpdated", new BookUpdatedDto(service.Message, _backToDisplayController));

            //Error state
            service.CopyErrorsToModelState(ModelState, dto);
            return View(dto);
        }

        public async Task<IActionResult> SoftDelete(int id, [FromServices] ICrudServicesAsync<BookDbContext> service)
        {
            Request.ThrowErrorIfNotLocal();
            var dto = new AlterSoftDeleteDto { BookId = id, SoftDeleted = true };
            await service.UpdateAndSaveAsync(dto);
            SetupTraceInfo();

            return View("BookUpdated", new BookUpdatedDto(
                service.IsValid ? "Successfully (soft) deleted the book" : service.GetAllErrors(),
                _backToDisplayController));
        }

        public async Task<IActionResult> ListSoftDeleted([FromServices] ICrudServicesAsync<BookDbContext> service)
        {
            Request.ThrowErrorIfNotLocal();

            var softDeletedBooks = await service.ProjectFromEntityToDto<Book, SimpleBookList>
                (x => x.IgnoreQueryFilters().Where(y => y.SoftDeleted)).ToListAsync();

            SetupTraceInfo();
            return View(softDeletedBooks);
        }

        public async Task<IActionResult> UnSoftDelete(int id, [FromServices] BookDbContext context)
        {
            Request.ThrowErrorIfNotLocal();
            var softDeletedBook = await context.Books.IgnoreQueryFilters().SingleAsync(x => x.BookId == id);
            softDeletedBook.AlterSoftDelete(false);
            await context.SaveChangesAsync();
            SetupTraceInfo();
            return View("BookUpdated", new BookUpdatedDto("Successfully un (soft) deleted the book", _backToDisplayController));
        }

        //------------------------------------------------------------
        //Admin parts

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
            var notes = await service.RunCheckAsync(scanFrom, dto.FixBadCacheValues, cancellationToken );
            SetupTraceInfo();
            return View("CacheCheckFixResult", notes);
        }

        public IActionResult GetTimingLogs()
        {
            var timingLogs = HttpTimingLog.GetTimingStats(5);
            return View(timingLogs);
        }

    }
}