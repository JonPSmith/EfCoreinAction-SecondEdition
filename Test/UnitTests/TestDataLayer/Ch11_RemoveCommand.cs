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
    public class Ch11_RemoveCommand
    {
        private readonly ITestOutputHelper _output;

        public Ch11_RemoveCommand(ITestOutputHelper output)
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
                var entity = new MyEntity {OneToOneOptional = new OneEntityOptional()};
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                context.MyEntities.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestRemoveNoRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new MyEntity ();
                context.Add(entity);
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter11DbContext(options))
            {
                var entity = context.MyEntities.Single();
                context.Remove(entity);
                context.SaveChanges();

                //VERIFY
                context.MyEntities.Count().ShouldEqual(0);
            }
        }

        [Fact]
        public void TestChangeTrackingDeleteNoRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new MyEntity();
                context.Add(entity);
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter11DbContext(options))
            {
                var entity = context.MyEntities.First();
                context.Remove(entity);
                context.SaveChanges();

                //VERIFY
                context.MyEntities.Count().ShouldEqual(0);
            }
        }

        [Fact]
        public void TestRemoveOptionalOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new MyEntity { OneToOneOptional = new OneEntityOptional() };
                context.Add(entity);
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter11DbContext(options))
            {
                var entity = context.MyEntities.Include(x => x.OneToOneOptional).Single();
                context.Remove(entity);
                context.SaveChanges();

                //VERIFY
                context.Entry(entity).State.ShouldEqual(EntityState.Detached);
                context.MyEntities.Count().ShouldEqual(0);
                context.OneEntities.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestRemoveRequiredOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new MyEntity { OneEntityRequired = new OneEntityRequired() };
                context.Add(entity);
                context.SaveChanges();
            }

            //ATTEMPT
            using (var context = new Chapter11DbContext(options))
            {
                var entity = context.MyEntities.Include(x => x.OneEntityRequired).Single();
                context.Remove(entity);
                context.SaveChanges();

                //VERIFY
                context.MyEntities.Count().ShouldEqual(0);
                context.Set<OneEntityRequired>().Count().ShouldEqual(0);
            }
        }

        [Fact]
        public void TestChangeTrackerDeleteOptionalOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new MyEntity { OneToOneOptional = new OneEntityOptional() };
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new Chapter11DbContext(options))
            {
                //ATTEMPT
                var entity = context.MyEntities.Include(x => x.OneToOneOptional).Single();
                context.Remove(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.Entry(entity).State.ShouldEqual(EntityState.Deleted);
                context.Entry(entity.OneToOneOptional).State.ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("OneToOneOptional");
                context.GetAllPropsNavsIsModified(entity.OneToOneOptional).ShouldEqual("MyEntityId");
            }
        }

        [Fact]
        public void TestChangeTrackerDeleteRequiredOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new MyEntity { OneEntityRequired = new OneEntityRequired() };
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = context.MyEntities.Include(x => x.OneEntityRequired).Single();
                context.Remove(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Deleted);
                context.GetEntityState(entity.OneEntityRequired).ShouldEqual(EntityState.Deleted);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("OneEntityRequired");
                context.GetAllPropsNavsIsModified(entity.OneEntityRequired).ShouldEqual("");
            }
        }

        [Fact]
        public void TestChangeTrackerDeleteRequiredWithNewOneToOneOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new MyEntity();
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = context.MyEntities.Single();
                entity.OneEntityRequired = new OneEntityRequired();
                context.Remove(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Deleted);
                context.GetEntityState(entity.OneEntityRequired).ShouldEqual(EntityState.Added);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("OneEntityRequired");
                context.GetAllPropsNavsIsModified(entity.OneEntityRequired).ShouldEqual("");
            }
        }


        [Fact]
        public void TestChangeTrackerDeleteTrackedOneRequiredOk()
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

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = 
                    context.MyEntities
                    .First();
                var oneToOne =
                    context.OneEntities
                    .First();
                entity.OneToOneOptional = oneToOne;
                context.Remove(entity);

                //VERIFY
                context.NumTrackedEntities().ShouldEqual(2);
                context.GetEntityState(entity).ShouldEqual(EntityState.Deleted);
                context.GetEntityState(oneToOne).ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("OneToOneOptional");
                context.GetAllPropsNavsIsModified(oneToOne).ShouldEqual("MyEntityId");
            }
        }
    }
}
