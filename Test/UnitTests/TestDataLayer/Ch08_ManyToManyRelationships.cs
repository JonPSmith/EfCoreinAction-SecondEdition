// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch08_ManyToManyRelationships
    {
        private ITestOutputHelper _output;

        public Ch08_ManyToManyRelationships(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestCreateBookWithTagsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var book = new Book
                {
                    Title = "Test",
                    Tags = new List<Tag> {new Tag {TagId = "T1"}, new Tag {TagId = "T2"}}
                };
                context.Add(book);
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(1);
                context.Tags.Count().ShouldEqual(2);
            }
        }


    }
}