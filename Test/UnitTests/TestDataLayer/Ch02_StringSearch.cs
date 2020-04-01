// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.DatabaseServices.Concrete;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch02_StringSearch
    {
        private readonly ITestOutputHelper _output;
        public Ch02_StringSearch(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestSqlServerCaseInsensitive()
        {
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            CheckCaseSensitivity(options, "SQL Server");
        }

        [Fact]
        public void TestSqliteCaseInsensitive()
        {
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            CheckCaseSensitivity(options, "SQL Server");
        }

        private void CheckCaseSensitivity(DbContextOptions<EfCoreContext> options, string databaseType)
        {
            using (var context = new EfCoreContext(options))
            {
                const string normalTitle = "Entity Framework in Action";
                if (context.Database.EnsureCreated())
                {
                    //new database created, so seed
                    var book1 = new Book {Title = normalTitle};
                    var book2 = new Book {Title = "entity FRAMEWORK in action"};
                    context.AddRange(book1, book2);
                    context.SaveChanges();
                }

                void OutputResult(IQueryable<Book> query, string type)
                {
                    var foundCount = query.Count();
                    var caseText = foundCount == 2 ? "INsensitive" : "sensitive";
                    _output.WriteLine($"{type} on {databaseType} is\t case-{caseText}. SQL created is:");
                    foreach (var line in query.ToQueryString().Split('\n').Select(x => x.Trim()))
                    {
                        _output.WriteLine("      " + line);
                    }
                }

                //ATTEMPT
                OutputResult(context.Books.Where(x => x.Title == normalTitle), "==");
                OutputResult(context.Books.Where(x => x.Title.StartsWith("Entity")), "StartsWith");
                OutputResult(context.Books.Where(x => x.Title.EndsWith("Action")), "EndsWith");
                OutputResult(context.Books.Where(x => x.Title.Contains("Framework")), "Contains");
                OutputResult(context.Books.Where(x => EF.Functions.Like(x.Title, normalTitle)), "Like");
            }
        }

    }
}