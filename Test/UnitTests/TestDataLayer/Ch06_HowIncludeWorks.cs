// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch06_HowIncludeWorks
    {
        private readonly ITestOutputHelper _output;

        public Ch06_HowIncludeWorks(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestBookQueryWithIncludes()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var book4Reviews2Authors = EfTestData.CreateDummyBooks(5).Last();
                context.Add(book4Reviews2Authors);
                context.SaveChanges();

                //ATTEMPT
                var query = context.Books
                    .Include(x => x.Reviews)
                    .Include(x => x.AuthorsLink)
                        .ThenInclude(x => x.Author);

                //VERIFY
                _output.WriteLine(query.ToQueryString());

            }
        }

        
    }
}