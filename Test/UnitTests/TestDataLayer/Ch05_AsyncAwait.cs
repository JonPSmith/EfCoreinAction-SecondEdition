// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.BookServices;
using ServiceLayer.BookServices.Concrete;
using Test.TestHelpers;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch05_AsyncAwait
    {
        [Fact]
        public async Task RunClientServerSimpleAsync()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var dtos = await context.Books
                    .Select(p => p.BookId.ToString() + "Hello").ToListAsync();

                //VERIFY
                dtos.Count.ShouldEqual(4);
            }
        }

        //This fails see https://github.com/aspnet/EntityFrameworkCore/issues/9570
        [Fact]
        public async Task RunClientServerCollectionAsync()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var dtos = await context.Books
                    .Select(p => 
                        string.Join(", ",
                            p.AuthorsLink
                                .Select(q => q.Author.Name))).ToListAsync();

                //VERIFY
                dtos.Count.ShouldEqual(4);
            }
        }

        [Fact]
        public async Task RunSelectAverageAsync()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var dtos = await context.Books
                    .Select(p => (double?)p.Reviews
                                    .Average(q => q.NumStars)
                        ).ToListAsync();

                //VERIFY
                dtos.Count.ShouldEqual(4);
            }
        }

        [Fact]
        public async Task RunBookListAsync()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var service = new ListBooksService(context);

                //ATTEMPT
                var books = await service.SortFilterPage(new SortFilterPageOptions()).ToListAsync();

                //VERIFY
                books.Count.ShouldEqual(4);
            }
        }

        public async Task<int> //#A
            GetNumBooksAsync(EfCoreContext context) //#B
        {
            return await //#C
                context.Books
                .CountAsync(); //#D
        }
        /**********************************************
        #A The 'async Task<T>' is the way we define a method as having an await in it
        #B Async methods names end, by convention, with Async
        #C The await keyword tells the compiler to place a task call, with a callback to continue after it has finished
        #D The CountAsync is an EF Core method that returns the count of the query, in this case the count of all the books in the database
         * *********************************************/

        private Task<int> GetNumBooksTask(EfCoreContext context)
        {
            return context.Books.CountAsync();
        }
    }
}