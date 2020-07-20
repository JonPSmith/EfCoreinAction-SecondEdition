// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using Test.Chapter10Listings.EfClasses;
using Test.Chapter10Listings.EfCode;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using TestSupportSchema;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch10_DefaultValues
    {
        [Fact]
        public void TestDefaultConstantValueOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureClean();

                //ATTEMPT
                var entity = new DefaultTest { Name = "Unit Test" };
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                entity.DateOfBirth.ShouldEqual(new DateTime(2000, 1, 1));
            }
        }

        [Fact]
        public void TestDefaultConstantValueOverriddenOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureClean();

                //ATTEMPT
                var entity = new DefaultTest { Name = "Unit Test", DateOfBirth = new DateTime(2015,2,3)};
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                entity.DateOfBirth.ShouldEqual(new DateTime(2015, 2, 3));
            }
        }

        [Fact]
        public void TestDefaultConstantSqlOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureClean();

                //ATTEMPT
                var entity = new DefaultTest { Name = "Unit Test"};
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                entity.CreatedOn.Subtract(DateTime.UtcNow).Seconds.ShouldBeInRange(-3,0);
            }
        }

        [Fact]
        public void TestValueGeneratorCalledOnAddOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureClean();

                //ATTEMPT
                var entity = new DefaultTest {Name = "Unit Test"};
                context.Add(entity);

                //VERIFY
                entity.OrderId.StartsWith("Unit Test-").ShouldBeTrue();
            }
        }

        [Fact]
        public void TestValueGeneratorOverriddenOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureClean();

                //ATTEMPT
                var entity = new DefaultTest { Name = "Unit Test", OrderId = "override"};
                context.Add(entity);

                //VERIFY
                entity.OrderId.ShouldEqual("override");
            }
        }
    }
}