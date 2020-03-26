// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using BookApp.HelperExtensions;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using EfCoreInAction.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceLayer.AdminServices;
using ServiceLayer.DatabaseServices.Concrete;

namespace BookApp.Controllers
{
    public class AdminController : BaseTraceController
    {
        private readonly IServiceProvider _serviceProvider; //#A

        public AdminController(
            IServiceProvider serviceProvider) //#B
        {
            _serviceProvider = serviceProvider; //#C
        }
        /******************************************************************
        #A This holds a local copy of the DI service provider
        #B I use constructor injection to get the service provider
        #C I copy the instance provided by DI into my private field
        ***********************************************************************/

        //-----------------------------------------------------
        //example task running

        public async Task<IActionResult> RunTaskWait() //#A
        {
            var scopeFactory = _serviceProvider
                .GetRequiredService<IServiceScopeFactory>(); //#B

            var task1 = MyTask(scopeFactory, 10); //#C
            var task2 = MyTask(scopeFactory, 20); //#C
            var results = await //#D
                Task.WhenAll(task1, task2); //#E

            SetupTraceInfo(); //!!!!!!!!!!!!!!!! REMOVE THIS FOR BOOK as not obvious
            return View(results); //#F
        }
        /******************************************************************
        #A This is the ASP.NET action method that is going to run both tasks in parallel
        #B I ask the DI service provider for a ServiceScopeFactory
        #C I define two tasks, each is given the ServiceScopeFactory
        #D In this case I want to wait until all the tasks have finished
        #E The Task.WhenAll method runs all the tasks it has been given in parallel and only returns when both are finished. It returns an array of results, one entry from each task
        #D I return the results to the user
         * ****************************************************************/

        public IActionResult RunTaskBackground() //#A
        {
            var scopeFactory = _serviceProvider
                .GetRequiredService<IServiceScopeFactory>(); //#B

            const int bgDelayMs = 2000;
            var timer = new Stopwatch();
            timer.Start();
            Task.Run(() => MyTask(scopeFactory, bgDelayMs)); //#C
            timer.Stop();

            return View(new Tuple<int, int>(bgDelayMs, (int) timer.ElapsedMilliseconds)); //#D
        }
        /******************************************************************
        #A This is the ASP.NET action method that is going to kick off the task, and then return immediately
        #B I ask the DI service provider for a ServiceScopeFactory
        #C I run my task and pass in the scopeFactory for the task to use to get what it needs
        #D I return to the user without waiting for the task to finish
         * ****************************************************************/

        //thanks to http://stackoverflow.com/questions/39109234/dbcontext-for-background-tasks-via-dependency-injection
        private async Task<int> MyTask
        (IServiceScopeFactory scopeFactory, //#A
            int waitMilliseconds)
        {
            using (var serviceScope =
                scopeFactory.CreateScope()) //#B
            using (var context =
                serviceScope.ServiceProvider
                    .GetService<EfCoreContext>()) //#C
            {
                await Task.Delay(waitMilliseconds); //#D
                return await context.Books.CountAsync(); //#E         
            }
        }
        /***************************************************
        #A I pass in the service scope factory, which allows me to create a private scope
        #B I have created my own service scope - services will only last until the dispose of my service scope
        #C Now I can ask the service provider to create a local instance of the application's DbContext
        #D I call a delay to simulate some work
        #E Then I use the local application's DbContext to read the database
         * ************************************************/

        //-----------------------------------------------
        //Admin actions that are called on a sepcific book

        public IActionResult ChangePubDate //#A
        (int id, //#B
            [FromServices] IChangePubDateService service) //#C
        {
            Request.ThrowErrorIfNotLocal(); //REMOVE THIS FOR BOOK as it isn't relevant
            var dto = service.GetOriginal(id); //#D
            SetupTraceInfo(); //REMOVE THIS FOR BOOK as it could be confusing
            return View(dto); //#E
        }
        /**************************************************
        #A This is the action that is called if the user clicks the Admin->Change Pub Date link
        #B It receives the primary key of the book that the user wants to change
        #C This is where ASP.NET DI injects the ChangePubDateService instance
        #D Now we use the service to set up a dto to show the user
        #E This shows the user the page that allows them to edit the publication date
         * ************************************************/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePubDate(ChangePubDateDto dto,
            [FromServices] IChangePubDateService service)
        {
            Request.ThrowErrorIfNotLocal();
            service.UpdateBook(dto);
            SetupTraceInfo(); //REMOVE THIS FOR BOOK as it could be confusing
            return View("BookUpdated", "Successfully changed publication date");
        }

        public IActionResult ChangePromotion(int id, [FromServices] IChangePriceOfferService service)
        {
            Request.ThrowErrorIfNotLocal();

            var priceOffer = service.GetOriginal(id);
            ViewData["BookTitle"] = service.OrgBook.Title;
            ViewData["OrgPrice"] = service.OrgBook.Price < 0
                ? "Not currently for sale"
                : service.OrgBook.Price.ToString("c", new CultureInfo("en-US"));
            SetupTraceInfo();
            return View(priceOffer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePromotion(PriceOffer dto, [FromServices] IChangePriceOfferService service)
        {
            Request.ThrowErrorIfNotLocal();

            var book = service.UpdateBook(dto);
            SetupTraceInfo();
            return View("BookUpdated", "Successfully added/changed a promotion");
        }


        public IActionResult AddBookReview(int id, [FromServices] IAddReviewService service)
        {
            Request.ThrowErrorIfNotLocal();

            var review = service.GetBlankReview(id);
            ViewData["BookTitle"] = service.BookTitle;
            SetupTraceInfo();
            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBookReview(Review dto, [FromServices] IAddReviewService service)
        {
            Request.ThrowErrorIfNotLocal();

            var book = service.AddReviewToBook(dto);
            SetupTraceInfo();
            return View("BookUpdated", "Successfully added a review");
        }

        //------------------------------------------------
        //Amdin commands that are called from the top menu

        public async Task<IActionResult> ResetDatabase([FromServices] EfCoreContext context, [FromServices] IWebHostEnvironment env)
        {
            Request.ThrowErrorIfNotLocal();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            var numBooks = await context.SeedDatabaseIfNoBooksAsync(env.WebRootPath);
            SetupTraceInfo();
            return View("BookUpdated", $"Successfully reset the database and added {numBooks} books.");
        }
    }
}