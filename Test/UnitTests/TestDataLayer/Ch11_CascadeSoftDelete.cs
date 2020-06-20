// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.SoftDeleteServices.Concrete;
using Test.Chapter11Listings;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch11_CascadeSoftDelete
    {
        private ITestOutputHelper _output;

        public Ch11_CascadeSoftDelete(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestCreateEmployeeSoftDelOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CascadeSoftDelDbContext>();
            using (var context = new CascadeSoftDelDbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var ceo = EmployeeSoftDel.SeedEmployeeSoftDel(context);

                //VERIFY
                context.Employees.Count().ShouldEqual(11);
                EmployeeSoftDel.ShowHierarchical(ceo, x => _output.WriteLine(x));
            }
        }

        [Fact]
        public void TestCascadeDeleteEmployeeSoftDelOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CascadeSoftDelDbContext>();
            using (var context = new CascadeSoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var ceo = EmployeeSoftDel.SeedEmployeeSoftDel(context);

                //ATTEMPT
                context.Remove(ceo.WorksFromMe.First());
                context.SaveChanges();

                //VERIFY
                context.Employees.Count().ShouldEqual(4);
            }
        }

        [Fact]
        public void TestManualSoftDeleteEmployeeSoftDelOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CascadeSoftDelDbContext>();
            using (var context = new CascadeSoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var ceo = EmployeeSoftDel.SeedEmployeeSoftDel(context);

                //ATTEMPT
                ceo.WorksFromMe.First().SoftDeleteLevel = 1;
                context.SaveChanges();

                //VERIFY
                context.Employees.Count().ShouldEqual(10);
            }
        }


        [Fact]
        public void TestCascadeSoftDeleteEmployeeSoftDelOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CascadeSoftDelDbContext>();
            using (var context = new CascadeSoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var ceo = EmployeeSoftDel.SeedEmployeeSoftDel(context);

                var service = new CascadeSoftDelService(context);

                //ATTEMPT
                var numSoftDeleted = service.CascadeSoftDelete(ceo.WorksFromMe.First());

                //VERIFY
                numSoftDeleted.ShouldEqual(7);
                context.Employees.Count().ShouldEqual(4);
                context.Employees.IgnoreQueryFilters().Count().ShouldEqual(11);
                context.Employees.IgnoreQueryFilters().Select(x => x.SoftDeleteLevel).Where(x => x > 0).ToArray()
                    .ShouldEqual(new byte[]{1, 2, 2, 3,3,3,3});
            }
        }

        [Theory]
        [InlineData(true, 4)]
        [InlineData(false, 7)]
        public void TestCascadeSoftDeleteEmployeeSoftDelWithLoggingOk(bool nullNavigationalMeansNotLoaded, int selectCount)
        {
            //SETUP
            var logs = new List<string>();
            var options = SqliteInMemory.CreateOptionsWithLogging<CascadeSoftDelDbContext>(log => logs.Add(log.DecodeMessage()));
            using (var context = new CascadeSoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var ceo = EmployeeSoftDel.SeedEmployeeSoftDel(context);

                var service = new CascadeSoftDelService(context, nullNavigationalMeansNotLoaded);

                //ATTEMPT
                logs.Clear();
                var numSoftDeleted = service.CascadeSoftDelete(ceo.WorksFromMe.First());

                //VERIFY
                logs.Count(x => x.Contains("SELECT \"e\".\"EmployeeSoftDelId\", ")).ShouldEqual(selectCount);
                numSoftDeleted.ShouldEqual(7);
                context.Employees.Count().ShouldEqual(4);
                context.Employees.IgnoreQueryFilters().Count().ShouldEqual(11);
                context.Employees.IgnoreQueryFilters().Select(x => x.SoftDeleteLevel).Where(x => x > 0).ToArray()
                    .ShouldEqual(new byte[] { 1, 2, 2, 3, 3, 3, 3 });
            }
        }

        [Fact]
        public void TestCascadeSoftDeleteExistingSoftDeleteEmployeeSoftDelOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CascadeSoftDelDbContext>();
            using (var context = new CascadeSoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var ceo = EmployeeSoftDel.SeedEmployeeSoftDel(context);

                var service = new CascadeSoftDelService(context);
                var preNumSoftDeleted = service.CascadeSoftDelete(ceo.WorksFromMe.First().WorksFromMe.First());

                //ATTEMPT
                var numSoftDeleted = service.CascadeSoftDelete(ceo.WorksFromMe.First());

                //VERIFY
                preNumSoftDeleted.ShouldEqual(3);
                numSoftDeleted.ShouldEqual(4);
                context.Employees.Count().ShouldEqual(4);
                context.Employees.IgnoreQueryFilters().Count().ShouldEqual(11);
                context.Employees.IgnoreQueryFilters().Select(x => x.SoftDeleteLevel).Where(x => x > 0).ToArray()
                    .ShouldEqual(new byte[] { 1, 1, 2, 2, 2, 3, 3 });
            }
        }

        [Fact]
        public void TestCircularLoopCascadeSoftDeleteEmployeeSoftDelOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CascadeSoftDelDbContext>();
            using (var context = new CascadeSoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var ceo = EmployeeSoftDel.SeedEmployeeSoftDel(context);
                var devEntry = context.Employees.Single(x => x.Name == "dev1a");
                devEntry.WorksFromMe = new List<EmployeeSoftDel>{ devEntry.Manager.Manager};

                var service = new CascadeSoftDelService(context);

                //ATTEMPT
                var numSoftDeleted = service.CascadeSoftDelete(context.Employees.Single(x => x.Name == "CTO"));

                //VERIFY
                numSoftDeleted.ShouldEqual(7);
                
            }
        }

        [Fact]
        public void TestCascadeSoftDeleteNonEntityOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CascadeSoftDelDbContext>();
            using (var context = new CascadeSoftDelDbContext(options))
            {
                var service = new CascadeSoftDelService(context);

                //ATTEMPT
                var numSoftDeleted = service.CascadeSoftDelete(new Book());

                //VERIFY
                numSoftDeleted.ShouldEqual(0);
            }
        }

        //---------------------------------------------------------
        //disconnected tests

        [Fact]
        public void TestDisconnectedCascadeSoftDeleteEmployeeSoftDelOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CascadeSoftDelDbContext>();
            using (var context = new CascadeSoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var ceo = EmployeeSoftDel.SeedEmployeeSoftDel(context);
            }
            using (var context = new CascadeSoftDelDbContext(options))
            {

                var service = new CascadeSoftDelService(context);

                //ATTEMPT
                var numSoftDeleted = service.CascadeSoftDelete(context.Employees.Single(x => x.Name == "CTO"));

                //VERIFY
                numSoftDeleted.ShouldEqual(7);
                context.Employees.Count().ShouldEqual(4);
                context.Employees.IgnoreQueryFilters().Count().ShouldEqual(11);
                context.Employees.IgnoreQueryFilters().Select(x => x.SoftDeleteLevel).Where(x => x > 0).ToArray()
                    .ShouldEqual(new byte[] { 1, 2, 2, 3, 3, 3, 3 });
            }
        }

    }
}