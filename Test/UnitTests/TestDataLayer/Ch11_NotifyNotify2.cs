// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

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
    public class Ch11_NotifyNotify2
    {
        private readonly ITestOutputHelper _output;

        public Ch11_NotifyNotify2(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestNotifySaveChangesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();

            context.Add(new NotifyEntity() { MyString = "Test" });
            context.SaveChanges();
            context.ChangeTracker.Clear();
            
            //ATTEMPT
            var entity = context.Notify.First();
            entity.MyString = "Changed";
            context.SaveChanges();

            //VERIFY
            var checkEntity = context.Notify.First();
            checkEntity.MyString.ShouldEqual("Changed");
            context.Entry(checkEntity).Property(nameof(NotifyEntity.MyString)).OriginalValue.ShouldEqual("Changed");
        }

        [Fact]
        public void TestNotify2SaveChangesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();
            context.Add(new Notify2Entity { MyString = "Test" });
            context.SaveChanges();
            context.ChangeTracker.Clear();
            
            //ATTEMPT
            var entity = context.Notify2.First();
            entity.MyString = "Changed";
            context.SaveChanges();
            context.ChangeTracker.Clear();
            
            //VERIFY
            var checkEntity = context.Notify2.First();
            checkEntity.MyString.ShouldEqual("Changed");
        }

        [Fact]
        public void TestNotifyModifiedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();
            context.Add(new NotifyEntity { MyString = "Test" });
            context.SaveChanges();
            context.ChangeTracker.Clear();
            
            //ATTEMPT
            var entity = context.Notify.First();
            entity.MyString = "Changed";

            //VERIFY
            var checkEntity = context.Notify.First();
            checkEntity.MyString.ShouldEqual("Changed");
            context.Entry(checkEntity).Property(nameof(NotifyEntity.MyString)).OriginalValue.ShouldEqual("Test");
        }

        [Fact]
        public void TestNotify2ModifiedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();
            context.Add(new Notify2Entity { MyString = "Test" });
            context.SaveChanges();
            context.ChangeTracker.Clear();
            
            //ATTEMPT
            var entity = context.Notify2.First();
            entity.MyString = "Changed";

            //VERIFY
            context.NumTrackedEntities().ShouldEqual(1);
            context.GetEntityState(entity).ShouldEqual(EntityState.Modified);
            context.GetAllPropsNavsIsModified(entity).ShouldEqual("MyString");
        }


        [Fact]
        public void TestNotifyHasOriginalValuesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();
            context.Add(new NotifyEntity() { MyString = "Test" });
            context.SaveChanges();
            context.ChangeTracker.Clear();
            
            //ATTEMPT
            var entity = context.Notify.First();
            entity.MyString = "Changed";

            //VERIFY
            var checkEntity = context.Notify.First();
            checkEntity.MyString.ShouldEqual("Changed");
            context.Entry(checkEntity).Property(nameof(NotifyEntity.MyString)).OriginalValue.ShouldEqual("Test");
        }

        [Fact]
        public void TestNotify2HasNoOriginalValuesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();
            context.Add(new Notify2Entity() { MyString = "Test" });
            context.SaveChanges();
            context.ChangeTracker.Clear();
            
            //ATTEMPT
            var entity =
                context.Notify2.First();
            entity.MyString = "Changed";
            var ex = Assert.Throws<InvalidOperationException>(() => context.Entry(entity)
                .Property(nameof(NotifyEntity.MyString)).OriginalValue.ShouldEqual("Test"));

            //VERIFY
            ex.Message.StartsWith(
                    "The original value for property 'Notify2Entity.MyString' cannot be accessed because it is not being tracked. ")
                .ShouldBeTrue();
        }
    }
}
