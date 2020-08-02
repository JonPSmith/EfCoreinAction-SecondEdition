// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter11Listings.EfCode;
using Test.Chapter11Listings.ProxyEfClasses;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch11_ProxyNotifyEntities
    {
        private readonly ITestOutputHelper _output;

        public Ch11_ProxyNotifyEntities(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestChangingNotificationsUpdateStringOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ProxyNotifyDbContext>(
                builder => builder.UseChangeTrackingProxies(false));
            using (var context = new ProxyNotifyDbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new ProxyMyEntity {MyString = "hello"};
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new ProxyNotifyDbContext(options))
            {
                //ATTEMPT
                var entity = context.ProxyMyEntities.Single();
                entity.MyString = "goodbye";

                //VERIFY1
                context.Entry(entity).State.ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString");
                context.Entry(entity).Property(nameof(ProxyMyEntity.MyString)).OriginalValue.ShouldEqual("hello");

                context.SaveChanges();
            }
            using (var context = new ProxyNotifyDbContext(options))
            {
                //VERIFY2
                var entity = context.ProxyMyEntities.Single();

                entity.MyString.ShouldEqual("goodbye");
            }
        }

        [Fact]
        public void TestChangingNotificationsUpdateOneToOneRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ProxyNotifyDbContext>(
                builder => builder.UseChangeTrackingProxies(false));
            using (var context = new ProxyNotifyDbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new ProxyMyEntity {MyString = "hello"};
                context.Add(entity);
                context.Add(new ProxyOptional());
                context.SaveChanges();
            }
            using (var context = new ProxyNotifyDbContext(options))
            {
                //ATTEMPT
                var entity = context.ProxyMyEntities.Single();
                entity.ProxyOptional = context.Set<ProxyOptional>().Single();

                //VERIFY1
                context.Entry(entity).State.ShouldEqual(EntityState.Unchanged);
                context.Entry(entity.ProxyOptional).State.ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity.ProxyOptional).ShouldEqual("ProxyMyEntityId");
                context.SaveChanges();
            }
            using (var context = new ProxyNotifyDbContext(options))
            {
                //VERIFY2
                var entity = context.ProxyMyEntities.Include(x => x.ProxyOptional).Single();

                entity.ProxyOptional.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestChangingNotificationsUpdateManyRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ProxyNotifyDbContext>(
                builder => builder.UseChangeTrackingProxies(false));
            using (var context = new ProxyNotifyDbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new ProxyMyEntity { MyString = "hello" };
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new ProxyNotifyDbContext(options))
            {
                //ATTEMPT
                var entity = context.ProxyMyEntities.Single();
                entity.Many.Add(new ProxyMany());

                //VERIFY1
                context.Entry(entity).State.ShouldEqual(EntityState.Unchanged);
                //context.Entry(entity.Many).State.ShouldEqual(EntityState.Modified);
                //context.GetAllPropsNavsIsModified(entity.Many).ShouldEqual("ProxyMyEntityId");
                context.SaveChanges();
            }
            using (var context = new ProxyNotifyDbContext(options))
            {
                //VERIFY2
                var entity = context.ProxyMyEntities.Include(x => x.Many).Single();

                entity.Many.Count.ShouldEqual(1);
            }
        }

        [Fact]
        public void TestChangingAndChangedNotificationsUpdateStringOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ProxyNotifyDbContext>(
                builder => builder.UseChangeTrackingProxies());
            using (var context = new ProxyNotifyDbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = context.CreateProxy<ProxyMyEntity>();
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new ProxyNotifyDbContext(options))
            {
                //ATTEMPT
                var entity = context.ProxyMyEntities.Single();
                entity.MyString = "goodbye";

                //VERIFY
                context.Entry(entity).State.ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString");
                Assert.Throws<InvalidOperationException>(() =>
                    context.Entry(entity).Property(nameof(ProxyMyEntity.MyString)).OriginalValue.ShouldEqual("hello"));
            }
        }
        
        [Fact]
        public void TestChangingAndChangedNotificationsUpdateOneToOneRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ProxyNotifyDbContext>(
                builder => builder.UseChangeTrackingProxies());
            using (var context = new ProxyNotifyDbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = context.CreateProxy<ProxyMyEntity>();
                entity.MyString = "hello";
                context.Add(entity);
                context.Add(context.CreateProxy<ProxyOptional>());
                context.SaveChanges();
            }
            using (var context = new ProxyNotifyDbContext(options))
            {
                //ATTEMPT
                var entity = context.ProxyMyEntities.Single();
                entity.ProxyOptional = context.Set<ProxyOptional>().Single();

                //VERIFY
                context.Entry(entity).State.ShouldEqual(EntityState.Unchanged);
                context.Entry(entity.ProxyOptional).State.ShouldEqual(EntityState.Modified);
                context.GetAllPropsNavsIsModified(entity.ProxyOptional).ShouldEqual("ProxyMyEntityId");
            }
        }

        [Fact]
        public void TestChangingAndChangedNotificationsUpdateManyRelationshipOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<ProxyNotifyDbContext>(
                builder => builder.UseChangeTrackingProxies());
            using (var context = new ProxyNotifyDbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = context.CreateProxy<ProxyMyEntity>();
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new ProxyNotifyDbContext(options))
            {
                //ATTEMPT
                var entity = context.ProxyMyEntities.Single();
                entity.Many.Add(context.CreateProxy<ProxyMany>());

                //VERIFY1
                context.Entry(entity).State.ShouldEqual(EntityState.Unchanged);
                //context.Entry(entity.Many).State.ShouldEqual(EntityState.Modified);
                //context.GetAllPropsNavsIsModified(entity.Many).ShouldEqual("ProxyMyEntityId");
                context.SaveChanges();
            }

            using (var context = new ProxyNotifyDbContext(options))
            {
                //VERIFY2
                var entity = context.ProxyMyEntities.Include(x => x.Many).Single();

                entity.Many.Count.ShouldEqual(1);
            }
        }


    }
}
