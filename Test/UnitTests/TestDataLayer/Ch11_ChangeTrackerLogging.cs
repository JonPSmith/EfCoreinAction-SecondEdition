// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using Test.Chapter11Listings.EfClasses;
using Test.Chapter11Listings.EfCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch11_ChangeTrackerLogging
    {
        private readonly ITestOutputHelper _output;

        public Ch11_ChangeTrackerLogging(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestSetWhenNotCalledOk()
        {
            //SETUP
            var entity = new EntityAddUpdate();
 
            //ATTEMPT

            //VERIFY
            entity.WhenCreatedUtc.ShouldEqual(new DateTime());
            entity.CreatedBy.ShouldEqual(default);
            entity.LastUpdatedUtc.ShouldEqual(new DateTime());
            entity.LastUpdatedBy.ShouldEqual(default);
        }

        [Fact]
        public void TestEntityAddUpdateAddOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var entity = new EntityAddUpdate();
            context.Add(entity);
            context.SaveChanges();

            //VERIFY
            entity.WhenCreatedUtc.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-0.5, 0);
            entity.LastUpdatedUtc.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-0.5, 0);
        }

        [Fact]
        public void TestEntityAddUpdateUpdateOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            using var context = new Chapter11DbContext(options);
            context.Database.EnsureCreated();

            context.Add(new EntityAddUpdate());
            context.SaveChanges();
            Thread.Sleep(1000);
            context.ChangeTracker.Clear();
            
            //ATTEMPT
            var entity = context.LoggedEntities.First();
            entity.Name = "New Value";
            context.SaveChanges();
            context.ChangeTracker.Clear();
            
            //VERIFY
            var checkEntity = context.LoggedEntities.First();
            checkEntity.WhenCreatedUtc.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-1.5, -0.5);
            checkEntity.LastUpdatedUtc.Subtract(DateTime.UtcNow).TotalSeconds.ShouldBeInRange(-0.5, 0);
        }

    }
}
