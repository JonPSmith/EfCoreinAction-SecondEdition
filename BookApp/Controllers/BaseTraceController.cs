// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Logger;

namespace BookApp.Controllers
{
    public abstract class BaseTraceController : Controller
    {
        protected void SetupTraceInfo()
        {
            ViewData["TraceIdent"] = HttpContext.TraceIdentifier;
            ViewData["NumLogs"] = HttpRequestLog.GetHttpRequestLog(HttpContext.TraceIdentifier).RequestLogs.Count;
        }
    }
}