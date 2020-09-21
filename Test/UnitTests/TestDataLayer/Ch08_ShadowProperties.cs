// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter08Listings.EFCode;
using TestSupport.EfHelpers;
using TestSupportSchema;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch08_ShadowProperties
    {
        [Fact]
        public void TestShadowPropertyDeleteOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                var attendee = new Attendee
                {
                    Name = "Person1",
                    TicketOption1 = new TicketOption1 (),
                    Required = new RequiredTrack {Track = TrackNames.EfCore}
                };
                context.Add(attendee);
                context.SaveChanges();

                //ATTEMPT
                context.Remove(attendee);
                context.SaveChanges();

                //VERIFY
                context.Attendees.Count().ShouldEqual(0);
                context.Set<RequiredTrack>().Count().ShouldEqual(1);
            }
        }


        [Fact]
        public void TestShadowPropertyOptionalOk()
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
                    TicketOption1 = new TicketOption1 (),
                    Required = new RequiredTrack {Track = TrackNames.EfCore},
                    Optional = new OptionalTrack {Track = TrackNames.EfCore}
                };
                context.Add(attendee);
                context.SaveChanges();

                //VERIFY
                context.Set<RequiredTrack>().Count().ShouldEqual(1);
                context.Set<OptionalTrack>().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestShadowPropertyReplaceOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                var attendee = new Attendee
                {
                    Name = "Person1",
                    TicketOption1 = new TicketOption1 (),
                    Required = new RequiredTrack {Track = TrackNames.EfCore}
                };
                context.Add(attendee);
                context.SaveChanges();

                //ATTEMPT
                attendee.Required = new RequiredTrack {Track = TrackNames.AspNetCore};
                context.SaveChanges();

                //VERIFY
                context.Set<RequiredTrack>().Count().ShouldEqual(2);
            }
        }

        [Fact]
        public void TestShadowPropertyRequiredNotAddedOk()
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
                    TicketOption1 = new TicketOption1 ()
                };
                context.Add(attendee);
                //context.SaveChanges();
                var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.ShouldEqual(
                    "SQLite Error 19: 'FOREIGN KEY constraint failed'.");
                //context.Set<RequiredTrack>().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestShadowPropertyRequiredOk()
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
                    TicketOption1 = new TicketOption1 (),
                    Required = new RequiredTrack {Track = TrackNames.EfCore}
                };
                context.Add(attendee);
                context.SaveChanges();

                //VERIFY
                context.Set<RequiredTrack>().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestShadowPropertySqlOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureClean();

                //ATTEMPT
                var orgCount = context.Set<RequiredTrack>().Count();
                var attendee = new Attendee
                {
                    Name = "Person1",
                    TicketOption1 = new TicketOption1( ),
                    Required = new RequiredTrack { Track = TrackNames.EfCore }
                };
                context.Add(attendee);
                context.SaveChanges();

                //VERIFY
                context.Set<RequiredTrack>().Count().ShouldEqual(orgCount + 1);
            }
        }
    }
}