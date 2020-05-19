// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter06Listings;
using Test.Chapter07Listings;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch07_BackingFields
    {
        private readonly ITestOutputHelper _output;

        public Ch07_BackingFields(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestWriteEmptyPersonOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {

                context.Database.EnsureCreated();

                //ATTEMPT
                context.Add(new Person());
                context.SaveChanges();

                //VERIFY

            }
        }

        [Fact]
        public void TestMyPropertySetGetOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                //ATTEMPT
                var person = new Person {MyProperty = nameof(TestMyPropertySetGetOk) };
                context.Add(person);
                context.SaveChanges();
            }
            //VERIFY
            using (var context = new Chapter07DbContext(options))
            {
                context.People.First().MyProperty.ShouldEqual(nameof(TestMyPropertySetGetOk));
            }
        }

        [Fact]
        public void TestUpdatedOnSetGetOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            var now = DateTime.UtcNow;
            int personId;
            //ATTEMPT
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureCreated();

                var person = new Person { UpdatedOn =  now};
                context.Add(person);
                context.SaveChanges();
                personId = person.PersonId;
            }
            //VERIFY
            using (var context = new Chapter07DbContext(options))
            {
                context.People.Single(x => x.PersonId == personId).UpdatedOn.ShouldEqual(now);
                now.Kind.ShouldEqual(DateTimeKind.Utc);
                context.People.Where(x => x.PersonId == personId)
                    .Select(x => EF.Property<DateTime>(x, "UpdatedOn"))
                    .Single().Kind.ShouldEqual(DateTimeKind.Unspecified);
            }
        }

        [Fact]
        public void TestAutoPropertySetGetOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            //ATTEMPT
            using (var context = new Chapter07DbContext(options))
            {
                var person = new Person();
                person.SetAutoProperty(1234);
                context.Add(person);
                context.SaveChanges();
            }
            //VERIFY
            using (var context = new Chapter07DbContext(options))
            {
                context.People.First().AutoProperty.ShouldEqual(1234);
            }
        }

        [Fact]
        public void TestUpdatedOnQueryWithLogsOk()
        {
            //SETUP
            var showLog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<Chapter07DbContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.ToString());
            });
            var now = DateTime.UtcNow;
            int personId;
            using (var context = new Chapter07DbContext(options))
            {

                context.Database.EnsureCreated();

                //ATTEMPT
                showLog = true;
                var person = new Person { UpdatedOn = now };
                context.Add(person);
                context.SaveChanges();
                personId = person.PersonId;

                var peopleByUpdate = context.People.OrderBy(x => EF.Property<DateTime>(x, "UpdatedOn")).ToList();
            }
        }

        [Fact]
        public void TestDateOfBirthCalcOk()
        {
            //SETUP

            var tenYearsAgo = DateTime.Today.AddYears(-10).AddDays(-1);
            //ATTEMPT
            var person = new Person();
            person.SetDateOfBirth(tenYearsAgo);

            //VERIFY
            person.AgeYears.ShouldEqual(10);
        }

        [Fact]
        public void TestDateOfBirthSavedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            var tenYearsAgo = DateTime.Today.AddYears(-10).AddDays(-1);
            int personId;
            //ATTEMPT
            using (var context = new Chapter07DbContext(options))
            {
                var person = new Person();
                person.SetDateOfBirth(tenYearsAgo);
                context.Add(person);
                context.SaveChanges();
                personId = person.PersonId;
            }
            //VERIFY
            using (var context = new Chapter07DbContext(options))
            {
                context.People.Where(x => x.PersonId == personId)
                    .Select(x => EF.Property<DateTime>(x, "DateOfBirth"))
                    .Single().ShouldEqual(tenYearsAgo);
            }
        }
        
    }
}