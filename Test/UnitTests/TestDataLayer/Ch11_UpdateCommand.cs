// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
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
    public class Ch11_UpdateCommand
    {
        private readonly ITestOutputHelper _output;

        public Ch11_UpdateCommand(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestUpdateNewEntityWithGuidOkOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var entity = new OneEntityGuidOptional();
            context.Update(entity);
            context.SaveChanges();

            //VERIFY
            context.Set<OneEntityGuidOptional>().Count().ShouldEqual(1);
        }

        [Fact]
        public void TestUpdateNewEntitiesWithOptionalRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var entity = new MyEntity {OneToOneOptional = new OneEntityOptional()};
            context.Update(entity);
            context.SaveChanges();

            //VERIFY
            context.MyEntities.Count().ShouldEqual(1);
            context.OneOptionalEntities.Count().ShouldEqual(1);
        }


        [Fact]
        public void TestChangeTrackingUpdateNewEntitiesWithOptionalRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var entity = new MyEntity { OneToOneOptional = new OneEntityOptional() };
            context.Update(entity);

            //VERIFY
            context.NumTrackedEntities().ShouldEqual(2);
            context.Entry(entity).State.ShouldEqual(EntityState.Added);
            context.Entry(entity.OneToOneOptional).State.ShouldEqual(EntityState.Added);
            context.GetAllPropsNavsIsModified(entity).ShouldEqual("OneToOneOptional");
            context.GetAllPropsNavsIsModified(entity.OneToOneOptional).ShouldEqual("");
        }

        [Fact]
        public void TestUpdateNewEntitiesWithOptionalGuidRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var oneToOne = new OneEntityGuidOptional();
            context.Add(oneToOne);
            context.SaveChanges();
            var entity = new MyEntity { OneEntityGuidOptional = oneToOne };
            context.Update(entity);
            context.SaveChanges();

            //VERIFY
            context.MyEntities.Count().ShouldEqual(1);
            context.Set<OneEntityGuidOptional>().Count().ShouldEqual(1);
        }

        [Fact]
        public void TestTrackChangesUpdateNewEntitiesWithOptionalGuidRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var oneToOne = new OneEntityGuidOptional( );
            context.Add(oneToOne);
            context.SaveChanges();
            var entity = new MyEntity { OneEntityGuidOptional = oneToOne };
            context.Update(entity);

            //VERIFY
            context.NumTrackedEntities().ShouldEqual(2);
            context.Entry(entity).State.ShouldEqual(EntityState.Added);
            context.Entry(entity.OneEntityGuidOptional).State.ShouldEqual(EntityState.Modified);
            context.GetAllPropsNavsIsModified(entity).ShouldEqual("OneEntityGuidOptional");
            context.GetAllPropsNavsIsModified(entity.OneEntityGuidOptional).ShouldEqual("MyEntityId");
        }

        [Fact]
        public void TestUpdateLoadedEntitiesWithOptionalRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();
            context.Add(new MyEntity());
            context.SaveChanges();

            //ATTEMPT
            context.ChangeTracker.Clear();
            var entity = context.MyEntities.Single();
            entity.OneToOneOptional = new OneEntityOptional();
            context.Update(entity);
            context.SaveChanges();

            //VERIFY
            context.MyEntities.Count().ShouldEqual(1);
            context.OneOptionalEntities.Count().ShouldEqual(1);
        }

        [Fact]
        public void TestChangeTrackingUpdateLoadedEntitiesWithOptionalRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();
            context.Add(new MyEntity());
            context.SaveChanges();
            context.ChangeTracker.Clear();
            
            //ATTEMPT
            var entity = context.MyEntities.Single();
            entity.OneToOneOptional = new OneEntityOptional();
            context.Update(entity);

            //VERIFY
            context.NumTrackedEntities().ShouldEqual(2);
            context.Entry(entity).State.ShouldEqual(EntityState.Modified);
            context.Entry(entity.OneToOneOptional).State.ShouldEqual(EntityState.Added);
            context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString,OneToOneOptional");
            context.GetAllPropsNavsIsModified(entity.OneToOneOptional).ShouldEqual("");
        }

        [Fact]
        public void TestChangeTrackingUpdateReadInEntitiesWithOptionalRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();
            context.Add(new MyEntity());
            context.Add(new OneEntityOptional());
            context.SaveChanges();

            //ATTEMPT
            context.ChangeTracker.Clear();
            var entity = context.MyEntities.Single();
            entity.OneToOneOptional = context.OneOptionalEntities.Single();
            context.Update(entity);

            //VERIFY
            context.NumTrackedEntities().ShouldEqual(2);
            context.Entry(entity).State.ShouldEqual(EntityState.Modified);
            context.Entry(entity.OneToOneOptional).State.ShouldEqual(EntityState.Modified);
            context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString,OneToOneOptional");
            context.GetAllPropsNavsIsModified(entity.OneToOneOptional).ShouldEqual("MyEntityId");
        }

        [Fact]
        public void TestUpdateLoadedEntitiesWithRequiredRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();
            context.Add(new MyEntity());
            context.SaveChanges();

            //ATTEMPT
            context.ChangeTracker.Clear();
            var entity = context.MyEntities.Single();
            entity.OneEntityRequired = new OneEntityRequired();
            context.Update(entity);
            context.SaveChanges();

            //VERIFY
            context.MyEntities.Count().ShouldEqual(1);
            context.OneEntityRequired.Count().ShouldEqual(1);
        }

        [Fact]
        public void TestChangeTrackingUpdateLoadedEntitiesWithRequiredRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();
            context.Add(new MyEntity());
            context.SaveChanges();

            //ATTEMPT
            context.ChangeTracker.Clear();
            var entity = context.MyEntities.Single();
            entity.OneEntityRequired = new OneEntityRequired();
            context.Update(entity);

            //VERIFY
            context.NumTrackedEntities().ShouldEqual(2);
            context.Entry(entity).State.ShouldEqual(EntityState.Modified);
            context.Entry(entity.OneEntityRequired).State.ShouldEqual(EntityState.Added);
            context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString,OneEntityRequired");
            context.GetAllPropsNavsIsModified(entity.OneEntityRequired).ShouldEqual("");
        }

    }
}
