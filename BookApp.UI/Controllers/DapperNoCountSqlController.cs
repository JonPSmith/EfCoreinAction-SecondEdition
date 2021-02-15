// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Infrastructure.LoggingServices;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.DapperSql.Books.DapperCode;
using BookApp.ServiceLayer.DefaultSql.Books;
using BookApp.ServiceLayer.DisplayCommon.Books;
using BookApp.ServiceLayer.DisplayCommon.Books.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BookApp.UI.Controllers
{
    public class DapperNoCountSqlController : BaseTraceController
    {
        public async Task<IActionResult> Index(SortFilterPageOptionsNoCount options, [FromServices] BookDbContext context)
        {
            var bookList = (await context.DapperBookListQueryAsync(options)).ToList();
            options.SetupRestOfDto(bookList.Count);
            
            SetupTraceInfo();

            return View(new BookListNoCountCombinedDto(options, bookList));
        }

        /// <summary>
        /// This provides the filter search dropdown content
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetFilterSearchContent(SortFilterPageOptionsNoCount options, [FromServices] IBookFilterDropdownService service)
        {
            var traceIdent = HttpContext.TraceIdentifier;
            return Json(
                new TraceIndentGeneric<IEnumerable<DropdownTuple>>(
                    traceIdent,
                    service.GetFilterDropDownValues(
                        options.FilterBy)));
        }
    }
}