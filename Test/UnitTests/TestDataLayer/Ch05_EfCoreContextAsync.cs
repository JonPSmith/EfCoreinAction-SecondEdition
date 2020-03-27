// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.BookServices;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch05_EfCoreContextAsync
    {
        private readonly ITestOutputHelper _output;

        public Ch05_EfCoreContextAsync(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task TestWriteTestDataSqliteOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var books = EfTestData.CreateFourBooks();
                context.Books.AddRange(books);
                await context.SaveChangesAsync();

                //VERIFY
                context.Books.Count().ShouldEqual(4);
                context.Books.Count(p => p.Title.StartsWith("Quantum")).ShouldEqual(1);
            }
        }

        [Fact]
        public async Task TestReadSqliteAsyncOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                context.Books.AddRange(EfTestData.CreateFourBooks());
                await context.SaveChangesAsync();

                //ATTEMPT
                var books = await context.Books.ToListAsync();

                //VERIFY
                books.Count.ShouldEqual(4);
                books.Count(p => p.Title.StartsWith("Quantum")).ShouldEqual(1);
            }
        }

        [Fact]
        public async Task TestMapBookToDtoAsyncOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                context.Books.AddRange(EfTestData.CreateFourBooks());
                await context.SaveChangesAsync();

                //ATTEMPT
                var result = await context.Books.Select(p => 
                    new BookListDto
                    {
                        ActualPrice = p.Promotion == null
                            ? p.Price
                            : p.Promotion.NewPrice,
                        ReviewsCount = p.Reviews.Count,
                    }).ToListAsync();

                //VERIFY
                result.Count.ShouldEqual(4);
            }
        }
    }
}