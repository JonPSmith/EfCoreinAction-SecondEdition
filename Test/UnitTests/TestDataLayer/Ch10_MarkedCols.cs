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
    public class Ch10_MarkedCols
    {

        [Fact]
        public void TestGuidKeyNotSetOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureClean();

                //ATTEMPT

                var entity = new MyClass();
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                entity.MyClassId.ShouldEqual(new Guid());
            }
        }

        [Fact]
        public void TestGuidKeySetOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureClean();

                //ATTEMPT
                var key = Guid.NewGuid();
                var entity = new MyClass{MyClassId = key};
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                entity.MyClassId.ShouldEqual(key);
            }
        }

        [Fact]
        public void TestSecondaryKeyIsSetOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureClean();

                //ATTEMPT
                var entity = new MyClass { MyClassId = Guid.NewGuid() };
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                entity.SecondaryKey.ShouldNotEqual(0);
            }
        }


    }
}