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


        public IActionResult Index()
        {
            return View();
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