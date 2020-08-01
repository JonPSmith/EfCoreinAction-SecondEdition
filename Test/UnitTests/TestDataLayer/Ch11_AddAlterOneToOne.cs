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
    public class Ch11_AddAlterOneToOne
    {
        private readonly ITestOutputHelper _output;

        public Ch11_AddAlterOneToOne(ITestOutputHelper output)
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
                var track = new MyEntity { OneToOneOptional = new OneEntityOptional()};
                context.Add(track);
                var notify = new NotifyEntity {OneToOne = new NotifyOne()};
                context.Add(notify);
                context.SaveChanges();

                //VERIFY
                context.MyEntities.Count().ShouldEqual(1);
                context.Notify.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestModifyTrackedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new MyEntity ());
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter11DbContext(options))
            {
                var entity = context.MyEntities.Single();
                entity.OneToOneOptional = new OneEntityOptional();
                context.SaveChanges();

                //VERIFY
                context.MyEntities.Include(x => x.OneToOneOptional).Single().ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestModifyNotifyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new NotifyEntity());
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter11DbContext(options))
            {
                var entity = context.Notify.Single();
                entity.OneToOne = new NotifyOne();
                context.SaveChanges();

                //VERIFY
                context.Notify.Include(x => x.OneToOne).Single().ShouldNotBeNull();
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
                var oneToOne = new OneEntityOptional();
                entity.OneToOneOptional = oneToOne;
                context.Add(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetEntityState(entity.OneToOneOptional).ShouldEqual(EntityState.Added);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("OneToOneOptional");
            }
        }

        [Fact]
        public void TestChangeTrackerAddTrackedOneTrackedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new OneEntityOptional());
                context.SaveChanges();
            }

            using (var context = new Chapter11DbContext(options))
            {
                //ATTEMPT
                var entity = new MyEntity();
                var oneToOne = context.OneOptionalEntities.First();
                entity.OneToOneOptional = oneToOne;
                context.Add(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetEntityState(entity.OneToOneOptional).ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("OneToOneOptional");
                context.GetAllPropsNavsIsModified(entity.OneToOneOptional).ShouldEqual("MyEntityId");
            }
        }

        [Fact]
        public void TestAddNotTrackedOneTrackedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new OneEntityOptional());
                context.SaveChanges();
            }

            using (var context = new Chapter11DbContext(options))
            {
                //ATTEMPT
                var entity = new MyEntity();
                var oneToOne = context.OneOptionalEntities.AsNoTracking().First();
                entity.OneToOneOptional = oneToOne;
                context.Add(entity);
                var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.ShouldEqual("SQLite Error 19: 'UNIQUE constraint failed: OneOptionalEntities.Id'.");
            }
        }

        [Fact]
        public void TestChangeTrackerAddNotTrackedOneTrackedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new OneEntityOptional());
                context.SaveChanges();
            }

            using (var context = new Chapter11DbContext(options))
            {
                //ATTEMPT
                var entity = new MyEntity();
                var oneToOne = context.OneOptionalEntities.AsNoTracking().First();
                entity.OneToOneOptional = oneToOne;
                context.Add(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetEntityState(entity.OneToOneOptional).ShouldEqual(EntityState.Added);
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
                var entity = new NotifyEntity();
                entity.OneToOne = new NotifyOne();
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter11DbContext(options))
            {
                //VERIFY
                var entity = context.Notify.Include(x => x.OneToOne).First();

                entity.OneToOne.ShouldNotBeNull();
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
                context.Add(new MyEntity());
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter11DbContext(options))
            {
                var entity = context.MyEntities.Include(x => x.OneToOneOptional).Single();
                entity.OneToOneOptional = new OneEntityOptional();

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Unchanged);
                context.GetEntityState(entity.OneToOneOptional).ShouldEqual(EntityState.Added);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("OneToOneOptional");
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
                context.Add(new NotifyEntity());
                context.SaveChanges();
            }
            using (var context = new Chapter11DbContext(options))
            {
                //ATTEMPT
                var entity = context.Notify.Single();
                entity.OneToOne = new NotifyOne();
                context.SaveChanges();
            }
            using (var context = new Chapter11DbContext(options))
            {
                //VERIFY
                var entity = context.Notify.Include(x => x.OneToOne).Single();
                entity.OneToOne.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestChangeTrackerUpdateReplaceTrackedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                var track = new MyEntity();
                track.OneToOneOptional = new OneEntityOptional();
                context.Add(track);
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter11DbContext(options))
            {
                var entity = context.MyEntities.Include(x => x.OneToOneOptional).Single();
                var oldOneToOne = entity.OneToOneOptional;
                entity.OneToOneOptional = new OneEntityOptional();

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(3);
                context.GetEntityState(entity).ShouldEqual(EntityState.Unchanged);
                context.GetEntityState(entity.OneToOneOptional).ShouldEqual(EntityState.Added);
                context.GetEntityState(oldOneToOne).ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(oldOneToOne).ShouldEqual("MyEntityId");
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("OneToOneOptional");
            }
        }

        [Fact]
        public void TestChangeTrackerUpdateReplaceTrackedAfterSaveChangesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                var track = new MyEntity();
                track.OneToOneOptional = new OneEntityOptional();
                context.Add(track);
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter11DbContext(options))
            {
                var entity = context.MyEntities.Include(x => x.OneToOneOptional).Single();
                var oldOneToOne = entity.OneToOneOptional;
                entity.OneToOneOptional = new OneEntityOptional();
                context.SaveChanges();

                //VERIFY
                context.GetEntityState(entity.OneToOneOptional).ShouldEqual(EntityState.Unchanged);
                context.GetEntityState(oldOneToOne).ShouldEqual(EntityState.Unchanged);
                context.OneOptionalEntities.Count().ShouldEqual(2);
            }
        }

        [Fact]
        public void TestChangeTrackerUpdateReplaceExistingTrackedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new MyEntity());
                context.Add(new OneEntityOptional());
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter11DbContext(options))
            {
                var entity = context.MyEntities.Single();
                var existing = context.OneOptionalEntities.First();
                entity.OneToOneOptional = existing;

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Unchanged);
                context.GetEntityState(existing).ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("OneToOneOptional");
                context.GetAllPropsNavsIsModified(existing).ShouldEqual("MyEntityId");
            }
        }
    }
}
