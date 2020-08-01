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
    public class Ch11_AddAlterCollection
    {
        private readonly ITestOutputHelper _output;

        public Ch11_AddAlterCollection(ITestOutputHelper output)
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
                var track = new MyEntity();
                track.Many.Add(new ManyEntity());
                context.Add(track);
                var notify = new NotifyEntity();
                notify.Many.Add(new NotifyMany());
                context.Add(notify);
                context.SaveChanges();

                //VERIFY
                context.MyEntities.Count().ShouldEqual(1);
                context.Notify.Count().ShouldEqual(1);
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
                context.Add(new MyEntity ());
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter11DbContext(options))
            {
                var entity = context.MyEntities.Single();
                entity.Many.Add(new ManyEntity());
                context.SaveChanges();

                //VERIFY
                context.MyEntities.Single().Many.Count.ShouldEqual(1);
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
                context.Add(new NotifyEntity());
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter11DbContext(options))
            {
                var entity = context.Notify.Single();
                entity.Many.Add(new NotifyMany());
                context.SaveChanges();

                //VERIFY
                context.Notify.Single().Many.Count.ShouldEqual(1);
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
                entity.Many.Add(new ManyEntity());
                context.Add(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                context.GetEntityState(entity.Many.First()).ShouldEqual(EntityState.Added);
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
                entity.Many.Add(new NotifyMany());
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter11DbContext(options))
            {
                //VERIFY
                var entity = context.Notify.Include(x => x.Many).First();

                entity.Many.Count.ShouldEqual(1);
                //see https://github.com/dotnet/efcore/issues/21835
                //context.GetEntityState(entity).ShouldEqual(EntityState.Added);
                //context.GetEntityState(entity.Many.First()).ShouldEqual(EntityState.Added);
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

            using (var context = new Chapter11DbContext(options))
            {
                //ATTEMPT
                var entity = context.MyEntities.Include(x => x.Many).Single();
                var many = new ManyEntity();
                entity.Many.Add(many);
                context.SaveChanges();
            }
            using (var context = new Chapter11DbContext(options))
            {
                //VERIFY
                var entity = context.MyEntities.Include(x => x.Many).First();

                entity.Many.Count.ShouldEqual(1);
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
                entity.Many.Add(new NotifyMany());
                context.SaveChanges();
            }
            using (var context = new Chapter11DbContext(options))
            {
                //VERIFY
                var entity = context.Notify.Include(x => x.Many).First();

                entity.Many.Count.ShouldEqual(1);
            }
        }
    }
}
