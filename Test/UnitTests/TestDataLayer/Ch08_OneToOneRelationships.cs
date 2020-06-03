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
    public class Ch08_OneToOneRelationships
    {
        //this fails because of issue #8137 https://github.com/aspnet/EntityFramework/issues/8137
        [Fact]
        public void TestOption1OneToOneAddDuplicateTicketBad()
        {
            //SETUP
            int dupticketId;
            var options =SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var dupTicket = new Ticket {TicketType = Ticket.TicketTypes.Guest};
                var attendee = new Attendee {Name = "Person1", Ticket = dupTicket, Required = new RequiredTrack()};
                context.Add(attendee);
                context.SaveChanges();
                dupticketId = dupTicket.TicketId;
            }
            using (var context = new Chapter08DbContext(options))
            {
                var dupTicket = context.Tickets.Find(dupticketId);
                var attendee = new Attendee { Name = "Person1", Ticket = dupTicket, Required = new RequiredTrack() };
                context.Add(attendee);
                //context.SaveChanges();
                var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.ShouldEqual(
                    "SQLite Error 19: 'UNIQUE constraint failed: Attendees.TicketId'.");
                //context.Tickets.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestOption1OneToOneDeleteOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();
                var attendee = new Attendee
                {
                    Name = "Person1",
                    Ticket = new Ticket(),
                    Required = new RequiredTrack()
                };
                context.Add(attendee);
                context.SaveChanges();

                //ATTEMPT
                context.Remove(attendee);
                context.SaveChanges();

                //VERIFY
                context.Attendees.Count().ShouldEqual(0);
                context.Tickets.Count().ShouldEqual(1);
                context.Set<RequiredTrack>().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestOption1OneToOneDuplicateTicketBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var dupTicket = new Ticket {TicketType = Ticket.TicketTypes.Guest};
                var attendees = new List<Attendee>
                {
                    new Attendee {Name = "Person1", Ticket = dupTicket, Required = new RequiredTrack()},
                    new Attendee {Name = "Person2", Ticket = dupTicket, Required = new RequiredTrack()},
                };
                context.AddRange(attendees);
                context.SaveChanges();

                //VERIFY
                context.Tickets.Count().ShouldEqual(1);
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
                        Ticket = new Ticket {TicketType = Ticket.TicketTypes.Guest},
                        Required = new RequiredTrack()
                    },
                    new Attendee
                    {
                        Name = "Person2",
                        Ticket = new Ticket {TicketType = Ticket.TicketTypes.VIP},
                        Required = new RequiredTrack()
                    },
                    new Attendee
                    {
                        Name = "Person3",
                        Ticket = new Ticket {TicketType = Ticket.TicketTypes.Guest},
                        Required = new RequiredTrack()
                    },
                };
                context.AddRange(attendees);
                context.SaveChanges();

                //VERIFY
                context.Tickets.Count().ShouldEqual(3);
            }
        }

        //[Fact]
        //public void TestOption1OneToOneSqlOk()
        //{
        //    //SETUP
        //    var connection = this.GetUniqueDatabaseConnectionString();
        //    var optionsBuilder =
        //        new DbContextOptionsBuilder<Chapter08DbContext>();

        //    optionsBuilder.UseSqlServer(connection);
        //    using (var context = new Chapter08DbContext(optionsBuilder.Options))
        //    {
        //            context.Database.EnsureCreated();
        //            var orgTicketesCount = context.Tickets.Count();

        //            //ATTEMPT
        //            var attendees = new List<Attendee>
        //            {
        //                new Attendee
        //                {
        //                    Name = "Person1", Ticket = new Ticket{TicketType = Ticket.TicketTypes.Guest},
        //                    Required = new RequiredTrack()
        //                },
        //                new Attendee
        //                {
        //                    Name = "Person2", Ticket = new Ticket {TicketType = Ticket.TicketTypes.VIP },
        //                    Required = new RequiredTrack()
        //                },
        //                new Attendee
        //                {
        //                    Name = "Person3", Ticket = new Ticket{TicketType = Ticket.TicketTypes.Guest},
        //                    Required = new RequiredTrack()
        //                },
        //            };
        //            context.AddRange(attendees);
        //            context.SaveChanges();

        //            //VERIFY
        //            context.Tickets.Count().ShouldEqual(orgTicketesCount + 3);
        //    }
        //}
    }
}