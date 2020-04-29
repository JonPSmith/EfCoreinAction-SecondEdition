// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter01Listings;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch01_LambdaProperty
    {
        private ITestOutputHelper _output;

        public Ch01_LambdaProperty(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestPersonReadBackOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter01DbContext>();
            using (var context = new Chapter01DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new Person { FirstName = "John", LastName = "Doe" });
                context.SaveChanges();

                //ATTEMPT
                var person = context.Persons.First();

                //VERIFY
                person.FullName.ShouldEqual("John Doe");
            }
        }

        [Fact]
        public void TestFilterPersonViaFullPersonBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter01DbContext>();
            using (var context = new Chapter01DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new Person { FirstName = "John", LastName = "Doe" });
                context.SaveChanges();

                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => context.Persons.First(x => x.FullName == "John Doe"));

                //VERIFY
                ex.Message.ShouldContain("could not be translated.");
            }
        }

        [Fact]
        public void TestFilterPersonViaFullPersonOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter01DbContext>();
            using (var context = new Chapter01DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new Person { FirstName = "John", LastName = "Doe" });
                context.SaveChanges();

                //ATTEMPT
                var query = context.Persons.Where(x => x.FirstName + x.LastName == "JohnDoe");
                var person = query.First();

                //VERIFY
                person.ShouldNotBeNull();
                _output.WriteLine(query.ToQueryString());
            }
        }
    }
}