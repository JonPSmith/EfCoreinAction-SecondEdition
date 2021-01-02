// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter08Listings.EFCode;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch08_OneToOneShadowOption1
    {
        private ITestOutputHelper _output;

        public Ch08_OneToOneShadowOption1(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ListShadowAttendeeTicketOption1ColumnsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                context.ListPropertiesAndForeignKeys<ShadowAttendee>(_output);
                context.ListPropertiesAndForeignKeys<TicketOption1>(_output);

                //VERIFY
            }
        }

        [Fact]
        public void TestOption1OneToOneAddOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var shadowAttendee = new ShadowAttendee
                {
                    Name = "Person1",
                    TicketOption1 = new TicketOption1(),
                };
                context.Add(shadowAttendee);
                context.SaveChanges();

                //VERIFY
                context.ShadowAttendees.Count().ShouldEqual(1);
                context.TicketOption1s.Count().ShouldEqual(1);
                shadowAttendee.TicketOption1.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestOption1OneToOneNoTicketBad()
        {
            //SETUP
            using (var context = new Chapter08DbContext(
                SqliteInMemory.CreateOptions<Chapter08DbContext>()))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                context.Add(new ShadowAttendee {Name = "Person1"});
                var ex = Assert.Throws<InvalidOperationException>(() => context.SaveChanges());

                //VERIFY
                ex.Message.ShouldEqual("The value of 'ShadowAttendee.Id' is unknown when attempting to save changes. This is because the property is also part of a foreign key for which the principal entity in the relationship is not known.");
            }
        }

        [Fact]
        public void TestOption1OneToOneDeletePrincipalOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();
                var shadowAttendee = new ShadowAttendee
                {
                    Name = "Person1",
                    TicketOption1 = new TicketOption1(),
                    
                };
                context.Add(shadowAttendee);
                context.SaveChanges();

                //ATTEMPT
                context.Remove(context.TicketOption1s.Single());
                context.SaveChanges();

                //VERIFY
                context.TicketOption1s.Count().ShouldEqual(0);
                context.ShadowAttendees.Count().ShouldEqual(0);
            }
        }

        [Fact]
        public void TestOption1OneToOneDeleteDependentOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();
                var shadowAttendee = new ShadowAttendee
                {
                    Name = "Person1",
                    TicketOption1 = new TicketOption1(),
                    
                };
                context.Add(shadowAttendee);
                context.SaveChanges();

                //ATTEMPT
                context.Remove(context.ShadowAttendees.Single());
                context.SaveChanges();

                //VERIFY
                context.ShadowAttendees.Count().ShouldEqual(0);
                context.TicketOption1s.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestOption1OneToOneAddDuplicateTicketBad()
        {
            //SETUP
            int dupticketId;
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using var context = new Chapter08DbContext(options);
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var dupTicket = new TicketOption1();
                var shadowAttendee = new ShadowAttendee { Name = "Person1", TicketOption1 = dupTicket };
                context.Add(shadowAttendee);
                context.SaveChanges();
                dupticketId = dupTicket.TicketOption1Id;
            }
            context.ChangeTracker.Clear();
            
            {
                var dupTicket = context.TicketOption1s.Find(dupticketId);
                var newShadowAttendee = new ShadowAttendee { Name = "Person1", TicketOption1 = dupTicket };
                context.Add(newShadowAttendee);
                var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.ShouldEqual(
                    "SQLite Error 19: 'UNIQUE constraint failed: ShadowAttendees.Id'.");
                context.TicketOption1s.Count().ShouldEqual(1);
            }
        }



    }
}