// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Test.Chapter10Listings.EfClasses;
using Test.Chapter10Listings.EfCode;
using TestSupport.EfHelpers;
using TestSupportSchema;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch10_ComputedColumn
    {
        private readonly ITestOutputHelper _output;

        public Ch10_ComputedColumn(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestDynamicComputedColumnAddOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureClean();

                //ATTEMPT
                var entity = new Person();
                entity.SetDateOfBirth(new DateTime(2020, 1,1));
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                entity.YearOfBirth.ShouldEqual(2020);
                context.Entry(entity).Property(x => x.YearOfBirth).Metadata.ValueGenerated.ShouldEqual(ValueGenerated.OnAddOrUpdate);
            }
        }

        [Fact]
        public void TestDynamicComputedColumnUpdateOk()
        {
            //SETUP
            var showLog = false;
            var options = this.CreateUniqueClassOptionsWithLogging<Chapter10DbContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.ToString());
            });
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureClean();

                var entity = new Person { FirstName = "Unit", LastName = "Test" };
                entity.SetDateOfBirth(new DateTime(2020, 1, 1));
                context.Add(entity);
                context.SaveChanges();

                //ATTEMPT
                showLog = true;
                entity.SetDateOfBirth(new DateTime(2000, 1, 1));
                context.SaveChanges();

                //VERIFY
                entity.YearOfBirth.ShouldEqual(2000);
            }
        }

        [Fact]
        public void TestPersistentComputedColumnAddOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureClean();

                //ATTEMPT
                var entity = new Person { FirstName = "Unit", LastName = "Test" };
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                entity.FullName.ShouldEqual("Unit Test");
                context.Entry(entity).Property(x => x.YearOfBirth).Metadata.ValueGenerated.ShouldEqual(ValueGenerated.OnAddOrUpdate);
            }
        }

        [Fact]
        public void TestPersistentComputedColumnUpdateOk()
        {
            //SETUP
            var showLog = false;
            var options = this.CreateUniqueClassOptionsWithLogging<Chapter10DbContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.ToString());
            });
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureClean();
                var entity = new Person { FirstName = "Unit", LastName = "Test" };
                context.Add(entity);
                context.SaveChanges();

                //ATTEMPT
                showLog = true;
                entity.FirstName = "xUnit";
                context.SaveChanges();

                //VERIFY
                entity.FullName.ShouldEqual("xUnit Test");
                context.Entry(entity).Property(x => x.YearOfBirth).Metadata.ValueGenerated.ShouldEqual(ValueGenerated.OnAddOrUpdate);
            }
        }
    }
}