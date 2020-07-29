// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Test.Chapter11Listings.EfClasses;
using Test.Chapter11Listings.EfCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch11_ChangeTrackerEvents
    {
        private readonly ITestOutputHelper _output;

        public Ch11_ChangeTrackerEvents(ITestOutputHelper output)
        {
            _output = output;
        }

        private class StateChangedEventLog
        {
            public StateChangedEventLog(object entity, EntityStateChangedEventArgs args)
            {
                Entity = entity;
                Args = args;
            }

            public object Entity { get; }
            public EntityStateChangedEventArgs Args { get; }
        }

        [Fact]
        public void TestTrackedEventOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                var logs = new List<EntityTrackedEventArgs>();

                context.Database.EnsureCreated();
                context.ChangeTracker.Tracked += delegate(object sender, EntityTrackedEventArgs args)
                {
                    logs.Add(args);
                };

                //ATTEMPT
                var entity = new MyEntity {MyString = "Test"};
                context.Add(entity);

                //VERIFY
                logs.Count.ShouldEqual(1);
                logs.Single().FromQuery.ShouldBeFalse();
                logs.Single().Entry.Entity.ShouldEqual(entity);
            }
        }

        [Fact]
        public void TestStateChangedEventNoSaveChangesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                var logs = new List<StateChangedEventLog>();

                context.Database.EnsureCreated();
                context.ChangeTracker.StateChanged += delegate (object sender, EntityStateChangedEventArgs args)
                {
                    logs.Add(new StateChangedEventLog(sender, args));
                };

                //ATTEMPT
                var entity = new MyEntity { MyString = "Test" };
                context.Add(entity);

                //VERIFY
                logs.Count.ShouldEqual(0);
            }
        }

        [Fact]
        public void TestAddedStateChangedEventFromSaveChangesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                var logs = new List<StateChangedEventLog>();

                context.Database.EnsureCreated();
                context.ChangeTracker.StateChanged += delegate (object sender, EntityStateChangedEventArgs args)
                {
                    logs.Add(new StateChangedEventLog(sender, args));
                };

                //ATTEMPT
                var entity = new MyEntity { MyString = "Test" };
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                logs.Count.ShouldEqual(1);
                logs.Single().Args.OldState.ShouldEqual(EntityState.Added);
                logs.Single().Args.NewState.ShouldEqual(EntityState.Unchanged);
                logs.Single().Args.Entry.Entity.ShouldEqual(entity);
            }
        }

        [Fact]
        public void TestModifiedStateChangedEventAfterSaveChangesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new MyEntity { MyString = "Test" };
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter11DbContext(options))
            {
                var logs = new List<StateChangedEventLog>();

                context.ChangeTracker.StateChanged += delegate (object sender, EntityStateChangedEventArgs args)
                {
                    logs.Add(new StateChangedEventLog(sender, args));
                };

                //ATTEMPT
                var entity = context.MyEntities.Single();
                entity.MyString = "new name";
                context.SaveChanges();

                //VERIFY
                logs.Count.ShouldEqual(2);
                logs.First().Args.OldState.ShouldEqual(EntityState.Unchanged);
                logs.First().Args.NewState.ShouldEqual(EntityState.Modified);
                logs.Last().Args.OldState.ShouldEqual(EntityState.Modified);
                logs.Last().Args.NewState.ShouldEqual(EntityState.Unchanged);
            }
        }

        [Fact]
        public void TestModifiedStateChangedEventAfterEntryOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new MyEntity { MyString = "Test" };
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter11DbContext(options))
            {
                var logs = new List<StateChangedEventLog>();

                context.ChangeTracker.StateChanged += delegate (object sender, EntityStateChangedEventArgs args)
                {
                    logs.Add(new StateChangedEventLog(sender, args));
                };

                var entity = context.MyEntities.Single();
                entity.MyString = "new name";

                //ATTEMPT
                context.Entry(entity).State.ShouldEqual(EntityState.Modified);

                //VERIFY
                logs.Count.ShouldEqual(1);
                logs.Single().Args.OldState.ShouldEqual(EntityState.Unchanged);
                logs.Single().Args.NewState.ShouldEqual(EntityState.Modified);
            }
        }

    }
}
