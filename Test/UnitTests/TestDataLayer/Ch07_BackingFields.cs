// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
                context.Database.EnsureCreated();

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

        //THIS FAILS in EF Core 5, preview 4!
        [Fact]
        public void TestBackingFieldSetUpByDataAnnotationOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            int personId;
            //ATTEMPT
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureCreated();

                var person = new Person();
                person.SetPropertyAnnotationValue("some data");
                context.Add(person);
                context.SaveChanges();
                personId = person.PersonId;
            }
            //VERIFY
            using (var context = new Chapter07DbContext(options))
            {
                var query = context.People.Where(x => x.PersonId == personId);
                var entity = query.Single();

                _output.WriteLine(query.ToQueryString());
                entity.BackingFieldViaAnnotation.ShouldEqual("some data");
                context.People.Where(x => x.PersonId == personId)
                    .Select(x => EF.Property<string>(x, nameof(Person.BackingFieldViaAnnotation)))
                    .Single().ShouldEqual("some data");
            }
        }

        [Fact]
        public void TestBackingFieldSetUpByFluentApiOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            int personId;
            //ATTEMPT
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureCreated();

                var person = new Person();
                person.SetPropertyFluentValue("some data");
                context.Add(person);
                context.SaveChanges();
                personId = person.PersonId;
            }
            //VERIFY
            using (var context = new Chapter07DbContext(options))
            {
                var entity = context.People.Single(x => x.PersonId == personId);

                entity.BackingFieldViaFluentApi.ShouldEqual("some data");
                context.People.Where(x => x.PersonId == personId)
                    .Select(x => EF.Property<string>(x, nameof(Person.BackingFieldViaFluentApi)))
                    .Single().ShouldEqual("some data");
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
                context.Database.EnsureCreated();

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
                context.Database.EnsureCreated();

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
                    .Select(x => EF.Property<DateTime>(x, "_dateOfBirth"))
                    .Single().ShouldEqual(tenYearsAgo);
            }
        }
        
    }
}