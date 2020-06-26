// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.SoftDeleteServices.Concrete;
using Test.Chapter11Listings;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch11_HardDeleteCascadeSoftDelete
    {
        private ITestOutputHelper _output;

        public Ch11_HardDeleteCascadeSoftDelete(ITestOutputHelper output)
        {
            _output = output;
        }

        //---------------------------------------------------------
        //check

        [Fact]
        public void TestCheckCascadeSoftOfPreviousDeleteInfo()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CascadeSoftDelDbContext>();
            using (var context = new CascadeSoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var ceo = EmployeeSoftDel.SeedEmployeeSoftDel(context);

                var service = new CascadeSoftDelService(context);
                var numSoftDeleted = service.SetCascadeSoftDelete(context.Employees.Single(x => x.Name == "CTO")).NumFound;
                numSoftDeleted.ShouldEqual(7 + 6);
                EmployeeSoftDel.ShowHierarchical(ceo, x => _output.WriteLine(x), false);

                //ATTEMPT
                var info = service.CheckCascadeSoftDelete(context.Employees.IgnoreQueryFilters().Single(x => x.Name == "CTO"));

                //VERIFY
                info.NumFound.ShouldEqual(7 + 6);
                info.ToString().ShouldEqual("You would hard deleted an entity and its 12 dependents");
            }
        }

        [Fact]
        public void TestCheckCascadeSoftDeleteNoSoftDeleteInfo()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CascadeSoftDelDbContext>();
            using (var context = new CascadeSoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var ceo = EmployeeSoftDel.SeedEmployeeSoftDel(context);

                var service = new CascadeSoftDelService(context);

                //ATTEMPT
                var info = service.CheckCascadeSoftDelete(context.Employees.IgnoreQueryFilters().Single(x => x.Name == "ProjectManager1"));

                //VERIFY
                info.NumFound.ShouldEqual(0);
                info.ToString().ShouldEqual("No entries would be hard deleted");
            }
        }

        //---------------------------------------------------------
        //hard delete

        [Fact]
        public void TestHardDeleteCascadeSoftOfPreviousDeleteInfo()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CascadeSoftDelDbContext>();
            using (var context = new CascadeSoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var ceo = EmployeeSoftDel.SeedEmployeeSoftDel(context);

                var service = new CascadeSoftDelService(context);
                var numSoftDeleted = service.SetCascadeSoftDelete(context.Employees.Single(x => x.Name == "CTO")).NumFound;
                numSoftDeleted.ShouldEqual(7 + 6);
                EmployeeSoftDel.ShowHierarchical(ceo, x => _output.WriteLine(x), false);

                //ATTEMPT
                var info = service.HardDeleteSoftDeletedEntries(context.Employees.IgnoreQueryFilters().Single(x => x.Name == "CTO"));

                //VERIFY
                info.NumFound.ShouldEqual(7 + 6);
                info.ToString().ShouldEqual("You have hard deleted an entity and its 12 dependents");
                context.Employees.IgnoreQueryFilters().Count().ShouldEqual(4);
            }
        }

        [Fact]
        public void TestHardDeleteCascadeSoftDeleteNoSoftDeleteInfo()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CascadeSoftDelDbContext>();
            using (var context = new CascadeSoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var ceo = EmployeeSoftDel.SeedEmployeeSoftDel(context);

                var service = new CascadeSoftDelService(context);

                //ATTEMPT
                var info = service.HardDeleteSoftDeletedEntries(context.Employees.IgnoreQueryFilters().Single(x => x.Name == "ProjectManager1"));

                //VERIFY
                info.NumFound.ShouldEqual(0);
                info.ToString().ShouldEqual("No entries have been hard deleted");
            }
        }

    }
}