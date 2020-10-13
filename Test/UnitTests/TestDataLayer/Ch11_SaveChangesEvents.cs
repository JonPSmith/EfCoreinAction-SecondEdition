// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Test.Chapter11Listings.EfClasses;
using Test.Chapter11Listings.EfCode;
using Test.Chapter11Listings.Interfaces;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch11_SaveChangesEvents
    {
        private readonly ITestOutputHelper _output;

        public Ch11_SaveChangesEvents(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestSavingChangesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();

                context.SavingChanges += delegate
                (object dbContext,
                    SavingChangesEventArgs args)
                {
                    //VERIFY
                    context.ShouldBeSameAs(dbContext);
                    context.ChangeTracker.Entries().Single().State.ShouldEqual(EntityState.Added);
                };

                //ATTEMPT
                var entity = new MyEntity {MyString = "Test"};
                context.Add(entity);   
                context.SaveChanges();
            }
        }

        [Fact]
        public void TestSavedChangesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();

                context.SavedChanges += delegate
                (object dbContext,
                    SavedChangesEventArgs args)
                {
                    //VERIFY
                    context.ShouldBeSameAs(dbContext);
                    context.ChangeTracker.Entries().Single().State.ShouldEqual(EntityState.Unchanged);
                };

                //ATTEMPT
                var entity = new MyEntity { MyString = "Test" };
                context.Add(entity);
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task TestSavingChangesAsyncOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();

                context.SavingChanges += delegate
                (object dbContext,
                    SavingChangesEventArgs args)
                {
                    //VERIFY
                    context.ShouldBeSameAs(dbContext);
                    context.ChangeTracker.Entries().Single().State.ShouldEqual(EntityState.Added);
                };

                //ATTEMPT
                var entity = new MyEntity { MyString = "Test" };
                context.Add(entity);
                await context.SaveChangesAsync();
            }
        }

        [Fact]
        public void TestSaveChangesFailedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();

                context.SaveChangesFailed += delegate
                (object dbContext,
                    SaveChangesFailedEventArgs args)
                {

                };

                //ATTEMPT
                var entity = new MyEntity { MyString = "Test" };
                context.Add(entity);
                context.SaveChanges();
            }
        }

        [Fact]
        public void TestSavingChangesAlterOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new EntityAddUpdate { Name = "Test1" };
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter11DbContext(options))
            {
                context.SavingChanges += delegate
                    (object dbContext, 
                    SavingChangesEventArgs args)
                {
                    foreach (var entity in ((DbContext)dbContext)
                        .ChangeTracker.Entries()
                        .Where(e =>
                            e.State == EntityState.Added ||
                            e.State == EntityState.Modified))
                    {
                        var tracked = entity.Entity as ICreatedUpdated;
                        tracked?.LogChange(entity);
                    }

                };

                //ATTEMPT
                var updateEntity = context.LoggedEntities.Single();
                updateEntity.Name = "Test2";
                var addEntity = new EntityAddUpdate { Name = "Test3" };
                context.Add(addEntity);
                context.SaveChanges();

                //VERIFY
                updateEntity.WhenCreatedUtc.ShouldNotEqual(updateEntity.LastUpdatedUtc);
                addEntity.WhenCreatedUtc.ShouldEqual(addEntity.LastUpdatedUtc);
            }
        }


        [Fact]
        public void TestSavingChangesEventsLinkedToContextOk()
        {
            //SETUP
            var logs = new List<EntityEntry>();
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();

                context.SavingChanges += (dbContext, args) =>
                {
                    context.ChangeTracker.Entries().ToList().ForEach(x => logs.Add(x));
                };

                //ATTEMPT
                var entity = new MyEntity {MyString = "Test1"};
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter11DbContext(options))
            {
                var entity = new MyEntity { MyString = "Test2" };
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                logs.Count.ShouldEqual(1);
                ((MyEntity)logs.Single().Entity).MyString.ShouldEqual("Test1");
            }
        }

    }
}
