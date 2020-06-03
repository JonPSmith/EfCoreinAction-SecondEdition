// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter08Listings.EFCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch08_AlternateKey
    {
        [Fact]
        public void TestPersonNullAlternateKeyOk()
        {
            //SETUP
            using (var context = new Chapter08DbContext(SqliteInMemory.CreateOptions<Chapter08DbContext>()))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var person = new Person
                {
                    UserId = null,
                };
                var ex = Assert.Throws<InvalidOperationException>(() => context.Add(person));

                //VERIFY
                ex.Message.ShouldEqual(
                    "Unable to track an entity of type 'Person' because alternate key property 'UserId' is null. If the alternate key is not used in a relationship, then consider using a unique index instead. Unique indexes may contain nulls, while alternate keys must not.");
            }
        }

        [Fact]
        public void TestPersonOnlyOk()
        {
            //SETUP
            using (var context = new Chapter08DbContext(SqliteInMemory.CreateOptions<Chapter08DbContext>()))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var person = new Person
                {
                    UserId = "me@somewhere.com",
                };
                context.Add(person);
                context.SaveChanges();

                //VERIFY
                context.People.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestPersonWithContactInfoDeleteOk()
        {
            //SETUP
            using (var context = new Chapter08DbContext(
                SqliteInMemory.CreateOptions<Chapter08DbContext>()))
            {
                {
                    context.Database.EnsureCreated();

                    var person = new Person
                    {
                        UserId = "me@somewhere.com",
                        ContactInfo = new ContactInfo { MobileNumber = "12345" }
                    };
                    context.Add(person);
                    context.SaveChanges();

                    //ATTEMPT
                    context.Remove(person);
                    context.SaveChanges();

                    //VERIFY
                    context.People.Count().ShouldEqual(0);
                    context.Set<ContactInfo>().Count().ShouldEqual(0);
                }
            }
        }

        [Fact]
        public void TestPersonWithContactInfoOk()
        {
            //SETUP
            using (var context = new Chapter08DbContext(
                SqliteInMemory.CreateOptions<Chapter08DbContext>()))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var person = new Person
                {
                    UserId = "me@somewhere.com",
                    ContactInfo = new ContactInfo {MobileNumber = "12345"}
                };
                context.Add(person);
                context.SaveChanges();

                //VERIFY
                person.ContactInfo.EmailAddress.ShouldEqual("me@somewhere.com");
            }
        }
    }
}