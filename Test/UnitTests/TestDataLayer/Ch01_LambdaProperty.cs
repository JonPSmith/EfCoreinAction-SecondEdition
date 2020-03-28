// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.Chapter01Listings;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch01_LambdaProperty
    {

        [Fact]
        public void TestPersonReadBackOkOk()
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
    }
}