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
    public class Ch11_UpdateCommand
    {
        private readonly ITestOutputHelper _output;

        public Ch11_UpdateCommand(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestUpdateNewEntitiesWithRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new MyEntity {OneToOneOptional = new OneEntityOptional()};
                context.Update(entity);
                context.SaveChanges();

                //VERIFY
                context.MyEntities.Count().ShouldEqual(1);
                context.OneOptionalEntities.Count().ShouldEqual(1);
            }
        }


        [Fact]
        public void TestChangeTrackingUpdateNewEntitiesWithRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
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
        }

        [Fact]
        public void TestUpdateLoadedEntitiesWithRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new MyEntity();
                context.Add(entity);
                context.SaveChanges();

                //ATTEMPT
            }
            using (var context = new Chapter11DbContext(options))
            {
                var entity = context.MyEntities.Single();
                entity.OneToOneOptional = new OneEntityOptional();
                context.Update(entity);
                context.SaveChanges();

                //VERIFY
                context.MyEntities.Count().ShouldEqual(1);
                context.OneOptionalEntities.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestChangeTrackingUpdateLoadedEntitiesWithRelationshipOk()
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
        }

    }
}
