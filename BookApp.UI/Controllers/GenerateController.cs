// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookApp.Infrastructure.Books.Seeding;
using BookApp.Persistence.CosmosDb.Books;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.UI.HelperExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace BookApp.UI.Controllers
{
    public class GenerateController : BaseTraceController
    {
        private BookDbContext _context;

        public GenerateController(BookDbContext context)
        {
            _context = context;
        }


        // GET
        public IActionResult Index()
        {
            var cosmosAvailable = HttpContext.RequestServices.GetService<CosmosDbContext>() != null;

            return View((BookCount: _context.Books.Count(), cosmosAvailable));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Books(int totalBooksNeeded, bool wipeDatabase,
            CancellationToken cancellationToken,
            [FromServices] IBookGenerator generator,
            [FromServices] IWebHostEnvironment env)
        {
            Request.ThrowErrorIfNotLocal();

            var timeTaken = await generator.WriteBooksAsync(env.WebRootPath, wipeDatabase, totalBooksNeeded, true, cancellationToken);

            SetupTraceInfo();

            return
                View((object)((cancellationToken.IsCancellationRequested ? "Cancelled" : "Successful") +
                              $" generate. Num books in database = {_context.Books.Count()}. Took {timeTaken:g}"));
        }

        [HttpPost]
        public ActionResult NumBooks()
        {
            var dbExists = (_context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists();
            var message = dbExists ? $"Num books = {_context.Books.Count()}" : "database being wiped.";
            return Content(message);
        }
    }
}