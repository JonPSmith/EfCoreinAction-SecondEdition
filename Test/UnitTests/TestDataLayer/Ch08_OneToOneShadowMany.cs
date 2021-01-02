// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

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
    public class Ch08_OneToOneShadowMany
    {
        private ITestOutputHelper _output;

        public Ch08_OneToOneShadowMany(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ListShadowAttendeeShadowAttendeeNoteColumnsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                context.ListPropertiesAndForeignKeys<ShadowAttendee>(_output);
                context.ListPropertiesAndForeignKeys<ShadowWithNotes>(_output);
                context.ListPropertiesAndForeignKeys<ShadowAttendeeNote>(_output);

                //VERIFY
            }
        }

        [Fact]
        public void TestShadowAttendeeNoteAddOk()
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
                    Notes = new List<ShadowAttendeeNote> {new ShadowAttendeeNote{Note= "test"}}
                };
                context.Add(shadowAttendee);
                context.SaveChanges();

                //VERIFY
                context.ShadowAttendees.Count().ShouldEqual(1);
                context.Set<ShadowAttendeeNote>().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestAddNoteOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var note = new ShadowAttendeeNote {Note = "test"};
                context.Add(note);
                context.SaveChanges();

                //VERIFY
                context.Set<ShadowAttendeeNote>().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestShadowAttendeeDeletePrincipalOk()
        {
            //SETUP
            //var options = this.CreateUniqueClassOptions<Chapter08DbContext>();
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using var context = new Chapter08DbContext(options);
            context.Database.EnsureCreated();
            var shadowAttendee = new ShadowAttendee
            {
                Name = "Person1",
                TicketOption1 = new TicketOption1(),
                Notes = new List<ShadowAttendeeNote> {new ShadowAttendeeNote {Note = "test"}}

            };
            context.Add(shadowAttendee);
            context.SaveChanges();

            context.ChangeTracker.Clear();
            
            //ATTEMPT
            context.Remove(context.TicketOption1s.Single());
            var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

            //VERIFY
            ex.InnerException.Message.ShouldEqual("SQLite Error 19: 'FOREIGN KEY constraint failed'.");
        }

        [Fact]
        public void TestShadowAttendeeDeleteDependentOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using var context = new Chapter08DbContext(options);
            context.Database.EnsureCreated();
            var shadowAttendee = new ShadowAttendee
            {
                Name = "Person1",
                TicketOption1 = new TicketOption1(),
                Notes = new List<ShadowAttendeeNote> { new ShadowAttendeeNote { Note = "test" } }

            };
            context.Add(shadowAttendee);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //ATTEMPT
            context.Remove(context.Set<ShadowAttendeeNote>().Single());
            context.SaveChanges();

            //VERIFY
            context.Set<ShadowAttendeeNote>().Count().ShouldEqual(0);
            context.ShadowAttendees.Count().ShouldEqual(1);
        }


        //--Shadow many

        [Fact]
        public void TestShadowWithManyNoteAddOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using var context = new Chapter08DbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var shadowAttendee = new ShadowWithNotes
            {
                Notes = new List<ShadowAttendeeNote> { new ShadowAttendeeNote { Note = "test" } }
            };
            context.Add(shadowAttendee);
            context.SaveChanges();

            //VERIFY
            context.ShadowWithManys.Count().ShouldEqual(1);
            context.Set<ShadowAttendeeNote>().Count().ShouldEqual(1);
        }

        [Fact]
        public void TestShadowWithManyDeletePrincipalOk()
        {
            //SETUP
            //var options = this.CreateUniqueClassOptions<Chapter08DbContext>();
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using var context = new Chapter08DbContext(options);
            //context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            var shadowAttendee = new ShadowWithNotes
            {
                Notes = new List<ShadowAttendeeNote> { new ShadowAttendeeNote { Note = "test" } }
            };
            context.Add(shadowAttendee);
            context.SaveChanges();

            context.ChangeTracker.Clear();
            
            //ATTEMPT
            context.Remove(context.ShadowWithManys.Single());
            var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

            //VERIFY
            ex.InnerException.Message.ShouldEqual("SQLite Error 19: 'FOREIGN KEY constraint failed'.");
        }

        [Fact]
        public void TestShadowWithManyDeleteDependentOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using var context = new Chapter08DbContext(options);
            context.Database.EnsureCreated();
            var shadowAttendee = new ShadowWithNotes
            {
                Notes = new List<ShadowAttendeeNote> { new ShadowAttendeeNote { Note = "test" } }
            };
            context.Add(shadowAttendee);
            context.SaveChanges();
            
            context.ChangeTracker.Clear();

            //ATTEMPT
            context.Remove(context.Set<ShadowAttendeeNote>().Single());
            context.SaveChanges();

            //VERIFY
            context.Set<ShadowAttendeeNote>().Count().ShouldEqual(0);
            context.ShadowWithManys.Count().ShouldEqual(1);
        }
    }
}