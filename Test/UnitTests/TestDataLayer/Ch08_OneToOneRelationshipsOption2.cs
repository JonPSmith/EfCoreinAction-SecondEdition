// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter08Listings.EFCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch08_OneToOneRelationshipsOption2
    {
        [Fact]
        public void TestOption2OneToOneAddOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var ticket = new TicketOption2
                {
                    Attendee = new Attendee{Name = "Person1", TicketOption1 = new TicketOption1(), Required = new RequiredTrack()}
                };
                context.Add(ticket);
                context.SaveChanges();

                //VERIFY
                context.Attendees.Count().ShouldEqual(1);
                context.TicketOption2s.Count().ShouldEqual(1);
                ticket.Attendee.ShouldNotBeNull();

                var x = context.Model.FindEntityType(typeof(TicketOption2));
            }
        }

        [Fact]
        public void TestOption2OneToOneChangeTicketOk()
        {
            //SETUP
            //var options = this.CreateUniqueClassOptions<Chapter08DbContext>();
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                var ticket = new TicketOption2
                {
                    Attendee = new Attendee
                        { Name = "Person1", TicketOption1 = new TicketOption1(), Required = new RequiredTrack() }
                };
                context.Add(ticket);
                context.SaveChanges();
            }
            using (var context = new Chapter08DbContext(options))
            {
                //ATTEMPT
                var existingAttendee = context.Attendees.Include(x => x.TicketOption2).Single();
                context.Remove(existingAttendee.TicketOption2);
                existingAttendee.TicketOption2 = new TicketOption2();
                context.SaveChanges();

                //VERIFY
                context.Attendees.Count().ShouldEqual(1);
                context.TicketOption2s.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestOption2OneToOneDeleteDependentOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();
                var ticket = new TicketOption2
                {
                    Attendee = new Attendee { Name = "Person1", TicketOption1 = new TicketOption1(), Required = new RequiredTrack() }
                };
                context.Add(ticket);
                context.SaveChanges();

                //ATTEMPT
                context.Remove(ticket);
                context.SaveChanges();

                //VERIFY
                context.TicketOption2s.Count().ShouldEqual(0);
                context.Attendees.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestOption2OneToOneDeletePrincipalOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();
                var ticket = new TicketOption2
                {
                    Attendee = new Attendee { Name = "Person1", TicketOption1 = new TicketOption1(), Required = new RequiredTrack() }
                };
                context.Add(ticket);
                context.SaveChanges();

                //ATTEMPT
                context.Remove(ticket.Attendee);
                context.SaveChanges();

                //VERIFY
                context.Attendees.Count().ShouldEqual(0);
                context.TicketOption2s.Count().ShouldEqual(0);
            }
        }

        [Fact]
        public void TestOption2OneToOneNoTicketOk()
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
                context.TicketOption2s.Count().ShouldEqual(0);
                attendee.TicketOption2.ShouldBeNull();
            }
        }


    }
}