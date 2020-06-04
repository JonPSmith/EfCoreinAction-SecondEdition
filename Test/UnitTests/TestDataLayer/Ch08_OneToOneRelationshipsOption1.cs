// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter08Listings.EFCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch08_OneToOneRelationshipsOption1
    {
        [Fact]
        public void TestOption1OneToOneAddOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var attendee = new Attendee
                {
                    Name = "Person1",
                    TicketOption1 = new TicketOption1(),
                    Required = new RequiredTrack()
                };
                context.Add(attendee);
                context.SaveChanges();

                //VERIFY
                context.Attendees.Count().ShouldEqual(1);
                context.TicketOption1s.Count().ShouldEqual(1);
                attendee.TicketOption1.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestOption1OneToOneDuplicateTicketOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var dupTicket = new TicketOption1();
                var attendees = new List<Attendee>
                {
                    new Attendee {Name = "Person1", TicketOption1 = dupTicket, Required = new RequiredTrack()},
                    new Attendee {Name = "Person2", TicketOption1 = dupTicket, Required = new RequiredTrack()},
                };
                context.AddRange(attendees);
                context.SaveChanges();

                //VERIFY
                context.TicketOption1s.Count().ShouldEqual(1);
                attendees.All(x => x.Required != null).ShouldBeTrue();
            }
        }

        [Fact]
        public void TestOption1OneToOneChangeTicketOk()
        {
            //SETUP
            //var options = this.CreateUniqueClassOptions<Chapter08DbContext>();
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                var attendee = new Attendee
                {
                    Name = "Person1",
                    TicketOption1 = new TicketOption1(),
                    Required = new RequiredTrack()
                };
                context.Add(attendee);
                context.SaveChanges();
            }
            using (var context = new Chapter08DbContext(options))
            {
                //ATTEMPT
                var existingAttendee = context.Attendees.Single();
                existingAttendee.TicketOption1 = new TicketOption1 { };
                context.SaveChanges();

                //VERIFY
                context.Attendees.Count().ShouldEqual(1);
                context.TicketOption1s.Count().ShouldEqual(2);
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
                context.Add(new Attendee {Name = "Person1", Required = new RequiredTrack()});
                var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.ShouldEqual("SQLite Error 19: 'FOREIGN KEY constraint failed'.");
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
                var attendee = new Attendee
                {
                    Name = "Person1",
                    TicketOption1 = new TicketOption1(),
                    Required = new RequiredTrack()
                };
                context.Add(attendee);
                context.SaveChanges();

                //ATTEMPT
                context.Remove(context.TicketOption1s.Single());
                context.SaveChanges();

                //VERIFY
                context.TicketOption1s.Count().ShouldEqual(0);
                context.Attendees.Count().ShouldEqual(0);
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
                var attendee = new Attendee
                {
                    Name = "Person1",
                    TicketOption1 = new TicketOption1(),
                    Required = new RequiredTrack()
                };
                context.Add(attendee);
                context.SaveChanges();

                //ATTEMPT
                context.Remove(context.Attendees.Single());
                context.SaveChanges();

                //VERIFY
                context.Attendees.Count().ShouldEqual(0);
                context.TicketOption1s.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestOption1OneToOneAddDuplicateTicketBad()
        {
            //SETUP
            int dupticketId;
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var dupTicket = new TicketOption1();
                var attendee = new Attendee { Name = "Person1", TicketOption1 = dupTicket, Required = new RequiredTrack() };
                context.Add(attendee);
                context.SaveChanges();
                dupticketId = dupTicket.TicketId;
            }
            using (var context = new Chapter08DbContext(options))
            {
                var dupTicket = context.TicketOption1s.Find(dupticketId);
                var newAttendee = new Attendee { Name = "Person1", TicketOption1 = dupTicket, Required = new RequiredTrack() };
                context.Add(newAttendee);
                var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.ShouldEqual(
                    "SQLite Error 19: 'UNIQUE constraint failed: Attendees.TicketId'.");
                context.TicketOption1s.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestOption1OneToOneOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var attendees = new List<Attendee>
                {
                    new Attendee
                    {
                        Name = "Person1",
                        TicketOption1 = new TicketOption1(),
                        Required = new RequiredTrack()
                    },
                    new Attendee
                    {
                        Name = "Person2",
                        TicketOption1 = new TicketOption1(),
                        Required = new RequiredTrack()
                    },
                    new Attendee
                    {
                        Name = "Person3",
                        TicketOption1 = new TicketOption1(),
                        Required = new RequiredTrack()
                    },
                };
                context.AddRange(attendees);
                context.SaveChanges();

                //VERIFY
                context.TicketOption1s.Count().ShouldEqual(3);
            }
        }


    }
}