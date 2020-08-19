// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using BookApp.Infrastructure.LoggingServices;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books.Dtos;
using BookApp.ServiceLayer.DefaultSql.Books.Services;
using BookApp.UI.HelperExtensions;
using BookApp.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BookApp.UI.Controllers
{
    public class HomeController : BaseTraceController
    {
        private readonly BookDbContext _context;

        public HomeController(BookDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(SortFilterPageOptions options)
        {
            var listService = new ListBooksService(_context);

            var bookList = await (await listService.SortFilterPageAsync(options))
                .ToListAsync();

            SetupTraceInfo();

            return View(new BookListCombinedDto(options, bookList));
        }

        /// <summary>
        /// This provides the filter search dropdown content
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetFilterSearchContent 
            (SortFilterPageOptions options) 
        {
            var service = new 
                BookFilterDropdownService(_context); 

            var traceIdent = HttpContext.TraceIdentifier; //REMOVE THIS FOR BOOK as it could be confusing

            return Json( 
                new TraceIndentGeneric<IEnumerable<DropdownTuple>>(
                    traceIdent,
                    service.GetFilterDropDownValues( 
                        options.FilterBy))); 
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            var isLocal = Request.IsLocal();
            return View(isLocal);
        }

        //public IActionResult Contact()
        //{
        //    ViewData["Message"] = "Your contact page.";

        //    return View();
        //}

        public IActionResult Error()
        {
            return View(new ErrorViewModel
                { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}