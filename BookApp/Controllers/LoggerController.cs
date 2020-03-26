using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Logger;

namespace EfCoreInAction.Controllers
{
    public class LoggerController : Controller
    {
        [HttpGet]
        public JsonResult GetLog(string traceIdentifier)
        {
            return Json(HttpRequestLog.GetHttpRequestLog(traceIdentifier));
        }
    }
}