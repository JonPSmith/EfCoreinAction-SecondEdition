// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using BookApp.Infrastructure.LoggingServices;
using BookApp.ServiceLayer.CosmosEf.Books;
using BookApp.ServiceLayer.DisplayCommon.Books;
using BookApp.ServiceLayer.DisplayCommon.Books.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BookApp.UI.Controllers
{
    public class CosmosEfController : BaseTraceController
    {
        public async Task<IActionResult> Index (SortFilterPageOptionsNoCount options, [FromServices] ICosmosEfListNoSqlBooksService service)
        {
            var output = await service.SortFilterPageAsync(options);
            SetupTraceInfo();
            return View(new CosmosEfBookListCombinedDto(options, output));              
        }

        /// <summary>
        /// This provides the filter search dropdown content
        /// </summary>
        /// <param name="options"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetFilterSearchContent    
            (SortFilterPageOptionsNoCount options, [FromServices]ICosmosEfBookFilterDropdownService service)         
        {
            var traceIdent = HttpContext.TraceIdentifier;
            var dropdown = await service.GetFilterDropDownValuesAsync(options.FilterBy);
            return Json(                            
                new TraceIndentGeneric<IEnumerable<DropdownTuple>>(
                traceIdent, dropdown));            
        }

    }
}
