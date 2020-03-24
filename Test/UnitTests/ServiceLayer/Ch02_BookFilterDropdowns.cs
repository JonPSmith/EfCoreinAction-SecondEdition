// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using ServiceLayer.BookServices.Concrete;
using ServiceLayer.BookServices.QueryObjects;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.ServiceLayer
{
    public class Ch02_BookFilterDropdowns
    {
        [Fact]
        public void DropdownByDate()
        {
            //SETUP
            const int numBooks = 5;
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.Books.AddRange(EfTestData.CreateDummyBooks(numBooks, true));
                context.SaveChanges();
                var service = new BookFilterDropdownService(context);

                //ATTEMPT
                var dropDown = service.GetFilterDropDownValues(BooksFilterBy.ByPublicationYear);

                //VERIFY
                dropDown.Select(x => x.Value).ToArray().ShouldEqual(new[] {"2014", "2013", "2012", "2011", "2010"});
            }
        }
    }
}