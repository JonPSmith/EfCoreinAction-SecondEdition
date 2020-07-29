// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter11Listings.EfClasses;
using Test.Chapter11Listings.EfCode;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch11_AddAlterScalar
    {
        private readonly ITestOutputHelper _output;

        public Ch11_AddAlterScalar(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestAddEntitiesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                context.Add(new MyEntity {MyString = "Test"});
                context.Add(new NotifyEntity { MyString = "Notify" });
                context.SaveChanges();

                //VERIFY
                context.MyEntities.Count().ShouldEqual(1);
                context.Notify.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestAddEntityTwiceOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new MyEntity {MyString = "Test"};
                context.Add(entity);
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                context.MyEntities.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestAddTrackedEntityOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new MyEntity { MyString = "Test" };
                context.Add(entity);
                context.SaveChanges();
                context.Add(entity);
                var ex = Assert.Throws< DbUpdateException> (() =>  context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.ShouldEqual("SQLite Error 19: 'UNIQUE constraint failed: MyEntities.Id'.");
            }
        }

        [Fact]
        public void TestUpdateTrackedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new MyEntity { MyString = "Test" });
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter11DbContext(options))
            {
                var entity = context.MyEntities.Single();
                entity.MyString = "Changed";
                context.SaveChanges();

                //VERIFY
                context.MyEntities.Single().MyString.ShouldEqual("Changed");
            }
        }

        [Fact]
        public void TestUpdateNotifyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new NotifyEntity { MyString = "Notify" });
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter11DbContext(options))
            {
                var entity = context.Notify.Single();
                entity.MyString = "Changed";
                context.SaveChanges();

                //VERIFY
                context.Notify.Single().MyString.ShouldEqual("Changed");
            }
        }

        [Fact]
        public void TestChangeTrackerAddTrackedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new MyEntity();
                entity.MyString = "Test";
                context.Add(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(1);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("");
            }
        }

        [Fact]
        public void TestChangeTrackerAddNotifyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new NotifyEntity {MyString = "Notify"};
                context.Add(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(1);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("");
            }
        }

        [Fact]
        public void TestChangeTrackerUpdateTrackedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new MyEntity { MyString = "Test" });
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter11DbContext(options))
            {
                var entity = context.MyEntities.Single();
                entity.MyString = "Changed";

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(1);
                context.GetEntityState(entity).ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString");
           }
        }

        [Fact]
        public void TestChangeTrackerUpdateNotifyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new NotifyEntity { MyString = "Notify" });
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter11DbContext(options))
            {
                var entity = context.Notify.Single();
                entity.MyString = "Changed";

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(1);
                context.GetEntityState(entity).ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString");
            }
        }
    }
}
