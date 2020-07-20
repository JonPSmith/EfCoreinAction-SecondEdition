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
    public class Ch10_ComputedColumn
    {

        [Fact]
        public void TestComputedColumnAddOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureClean();

                //ATTEMPT
                var entity = new Person{ Name = "Unit Test"};
                entity.SetDateOfBirth(new DateTime(2020, 1,1));
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                entity.YearOfBirth.ShouldEqual(2020);
            }
        }

        [Fact]
        public void TestComputedColumnUpdateOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureClean();

                var entity = new Person { Name = "Unit Test" };
                entity.SetDateOfBirth(new DateTime(2020, 1, 1));
                context.Add(entity);
                context.SaveChanges();

                //ATTEMPT
                entity.SetDateOfBirth(new DateTime(2000, 1, 1));
                context.SaveChanges();

                //VERIFY
                entity.YearOfBirth.ShouldEqual(2000);
            }
        }


    }
}