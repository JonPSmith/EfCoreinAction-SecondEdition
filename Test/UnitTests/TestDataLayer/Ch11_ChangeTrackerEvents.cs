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


        [Fact]
        public void TestTrackedEventOnAddOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();

            var logs = new List<EntityTrackedEventArgs>();  //#A
            context.ChangeTracker.Tracked += delegate(      //#B
                object sender, EntityTrackedEventArgs args) //#B
            {
                logs.Add(args);                             //#C
            };

            //ATTEMPT
            var entity = new MyEntity {MyString = "Test"};  //#D
            context.Add(entity);                            //#E

            //VERIFY
            logs.Count.ShouldEqual(1);                      //#F
            logs.Single().FromQuery.ShouldBeFalse();        //#G
            logs.Single().Entry.Entity.ShouldEqual(entity); //#H
            logs.Single().Entry.State                       //#I
                .ShouldEqual(EntityState.Added);            //#I
            /******************************************************************
                #A This will hold a log of any tracked events
                #B You register your event handler to the ChangeTracker.Tracked event
                #C This event handler simply logs the EntityTrackedEventArgs
                #D Create an entity class
                #E Add that entity class to context
                #F There is one event
                #G The entity wasn't tracking during a query
                #H You can access the entity that triggered the event
                #I You can also get the current State of that entity
                 ****************************************************************/
        }


        [Fact]
        public void TestTrackedEventOnQueryOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();
            context.Add(new MyEntity { MyString = "Test" });
            context.SaveChanges();
            context.ChangeTracker.Clear();
            {
                var trackedLogs = new List<EntityTrackedEventArgs>();
                context.ChangeTracker.Tracked += delegate (object sender, EntityTrackedEventArgs args)
                {
                    trackedLogs.Add(args);
                };

                //ATTEMPT
                var entity = context.MyEntities.Single();


                //VERIFY
                trackedLogs.Count.ShouldEqual(1);
                trackedLogs.Single().FromQuery.ShouldBeTrue();
                trackedLogs.Single().Entry.Entity.ShouldEqual(entity);
            }
        }

        [Fact]
        public void TestStateChangedEventNoSaveChangesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();

            var trackedLogs = new List<EntityTrackedEventArgs>();
            context.ChangeTracker.Tracked += delegate (object sender, EntityTrackedEventArgs args)
            {
                trackedLogs.Add(args);
            };

            var stateChangeLogs = new List<EntityStateChangedEventArgs>();
            context.ChangeTracker.StateChanged += delegate (object sender, EntityStateChangedEventArgs args)
            {
                stateChangeLogs.Add(args);
            };

            //ATTEMPT
            var entity = new MyEntity { MyString = "Test" };
            context.Add(entity);

            //VERIFY
            stateChangeLogs.Count.ShouldEqual(0);
            trackedLogs.Count.ShouldEqual(1);
            trackedLogs.Single().Entry.State.ShouldEqual(EntityState.Added);
        }


        [Fact]
        public void TestAddedStateChangedEventFromSaveChangesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();

            var logs = new List<EntityStateChangedEventArgs>();       //#A
            context.ChangeTracker.StateChanged += delegate            //#B
                (object sender, EntityStateChangedEventArgs args)     //#B
            {
                logs.Add(args);                                       //#C
            };

            //ATTEMPT
            var entity = new MyEntity { MyString = "Test" };          //#D
            context.Add(entity);                                      //#E
            context.SaveChanges();                                    //#F

            //VERIFY
            logs.Count.ShouldEqual(1);                                //#G
            logs.Single().OldState.ShouldEqual(EntityState.Added);    //#H
            logs.Single().NewState.ShouldEqual(EntityState.Unchanged);//#I
            logs.Single().Entry.Entity.ShouldEqual(entity);           //#J
            /******************************************************************
            #A This will hold a log of any StateChanged events
            #B You register your event handler to the ChangeTracker.StateChanged event
            #C This event handler simply logs the EntityTrackedEventArgs
            #D Create an entity class
            #E Add that entity class to context
            #F SaveChanges will change the State to Unchanged after the database update
            #G There is one event
            #H The State before the change was Added
            #I The State after the change is Unchanged
            #J You get access to the entity data via the Entry property
             ****************************************************************/
        }

        [Fact]
        public void TestAddedBothEventsFromSaveChangesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            var trackedLogs = new List<EntityTrackedEventArgs>();
            context.ChangeTracker.Tracked += delegate (object sender, EntityTrackedEventArgs args)
            {
                trackedLogs.Add(args);
            };

            var stateChangeLogs = new List<EntityStateChangedEventArgs>();
            context.Database.EnsureCreated();
            context.ChangeTracker.StateChanged += delegate (object sender, EntityStateChangedEventArgs args)
            {
                stateChangeLogs.Add(args);
            };

            //ATTEMPT
            var entity = new MyEntity { MyString = "Test" };
            context.Add(entity);
            context.SaveChanges();

            //VERIFY
            trackedLogs.Count.ShouldEqual(1);
            ((MyEntity)trackedLogs.Single().Entry.Entity).Id.ShouldNotEqual(0);
            stateChangeLogs.Count.ShouldEqual(1);
            stateChangeLogs.Single().OldState.ShouldEqual(EntityState.Added);
            stateChangeLogs.Single().NewState.ShouldEqual(EntityState.Unchanged);
            stateChangeLogs.Single().Entry.Entity.ShouldEqual(entity);
            ((MyEntity)stateChangeLogs.Single().Entry.Entity).Id.ShouldNotEqual(0);
        }

        [Fact]
        public void TestModifiedStateChangedEventAfterSaveChangesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();
            context.Add(new MyEntity { MyString = "Test" });
            context.SaveChanges();
            context.ChangeTracker.Clear();
            
            var logs = new List<EntityStateChangedEventArgs>();

            context.ChangeTracker.StateChanged += delegate(object sender, EntityStateChangedEventArgs args)
            {
                logs.Add(args);
            };

            //ATTEMPT
            var entity = context.MyEntities.Single();
            entity.MyString = "new name";
            context.SaveChanges();

            //VERIFY
            logs.Count.ShouldEqual(2);
            logs.First().OldState.ShouldEqual(EntityState.Unchanged);
            logs.First().NewState.ShouldEqual(EntityState.Modified);
            logs.Last().OldState.ShouldEqual(EntityState.Modified);
            logs.Last().NewState.ShouldEqual(EntityState.Unchanged);
        }

        [Fact]
        public void TestRemoveStateChangedEventNoSaveChangesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();
            context.Add(new MyEntity { MyString = "Test" });
            context.SaveChanges();
            context.ChangeTracker.Clear();
            
            var logs = new List<EntityStateChangedEventArgs>();

            context.ChangeTracker.StateChanged += delegate(object sender, EntityStateChangedEventArgs args)
            {
                logs.Add(args);
            };

            //ATTEMPT
            var entity = context.MyEntities.Single();
            context.Remove(entity);

            //VERIFY
            logs.Count.ShouldEqual(1);
            logs.First().OldState.ShouldEqual(EntityState.Unchanged);
            logs.First().NewState.ShouldEqual(EntityState.Deleted);
        }

        [Fact]
        public void TestModifiedStateChangedEventAfterEntryOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();
            context.Add(new MyEntity { MyString = "Test" });
            context.SaveChanges();
            context.ChangeTracker.Clear();
            
            var logs = new List<EntityStateChangedEventArgs>();

            context.ChangeTracker.StateChanged += delegate(object sender, EntityStateChangedEventArgs args)
            {
                logs.Add(args);
            };

            var entity = context.MyEntities.Single();
            entity.MyString = "new name";

            //ATTEMPT
            context.Entry(entity).State.ShouldEqual(EntityState.Modified);

            //VERIFY
            logs.Count.ShouldEqual(1);
            logs.Single().OldState.ShouldEqual(EntityState.Unchanged);
            logs.Single().NewState.ShouldEqual(EntityState.Modified);
        }

    }
}
