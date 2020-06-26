// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.SoftDeleteServices.Concrete;
using Test.Chapter11Listings;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch11_ResetCascadeSoftDelete
    {
        private ITestOutputHelper _output;
        private readonly Regex _selectMatchRegex = new Regex(@"SELECT "".""\.""EmployeeSoftDelId"",", RegexOptions.None);

        public Ch11_ResetCascadeSoftDelete(ITestOutputHelper output)
        {
            _output = output;
        }

        //---------------------------------------------------------
        //reset 

        [Fact]
        public void TestResetCascadeSoftOfPreviousDeleteOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CascadeSoftDelDbContext>();
            using (var context = new CascadeSoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var ceo = EmployeeSoftDel.SeedEmployeeSoftDel(context);

                var service = new CascadeSoftDelService(context);
                var numSoftDeleted = service.SetCascadeSoftDelete(context.Employees.Single(x => x.Name == "CTO"));
                numSoftDeleted.ShouldEqual(7+6);
                EmployeeSoftDel.ShowHierarchical(ceo, x => _output.WriteLine(x), false);

                //ATTEMPT
                var numUnSoftDeleted = service.ResetCascadeSoftDelete(context.Employees.IgnoreQueryFilters().Single(x => x.Name == "CTO"));

                //VERIFY
                numUnSoftDeleted.ShouldEqual(7+6);
                context.Employees.Count().ShouldEqual(11);
                context.Contracts.Count().ShouldEqual(9);
            }
        }

        [Fact]
        public void TestResetCascadeSoftDeletePartialOfPreviousDeleteDoesNothingOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CascadeSoftDelDbContext>();
            using (var context = new CascadeSoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var ceo = EmployeeSoftDel.SeedEmployeeSoftDel(context);

                var service = new CascadeSoftDelService(context);
                var numSoftDeleted = service.SetCascadeSoftDelete(context.Employees.Single(x => x.Name == "CTO"));
                numSoftDeleted.ShouldEqual(7 + 6);
                EmployeeSoftDel.ShowHierarchical(ceo, x => _output.WriteLine(x), false);

                //ATTEMPT
                var numUnSoftDeleted = service.ResetCascadeSoftDelete(context.Employees.IgnoreQueryFilters().Single(x => x.Name == "ProjectManager1"));

                //VERIFY
                EmployeeSoftDel.ShowHierarchical(ceo, x => _output.WriteLine(x), false);
                numUnSoftDeleted.ShouldEqual(0);
            }
        }

        [Fact]
        public void TestResetCascadeSoftDeleteTwoLevelSoftDeleteThenUndeleteTopOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CascadeSoftDelDbContext>();
            using (var context = new CascadeSoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var ceo = EmployeeSoftDel.SeedEmployeeSoftDel(context);

                var service = new CascadeSoftDelService(context);
                var numInnerSoftDelete = service.SetCascadeSoftDelete(context.Employees.Single(x => x.Name == "ProjectManager1"));
                numInnerSoftDelete.ShouldEqual(3 + 3);
                var numOuterSoftDelete = service.SetCascadeSoftDelete(context.Employees.Single(x => x.Name == "CTO"));
                numOuterSoftDelete.ShouldEqual(4 + 3);
                EmployeeSoftDel.ShowHierarchical(ceo, x => _output.WriteLine(x), false);

                //ATTEMPT
                var numUnSoftDeleted = service.ResetCascadeSoftDelete(context.Employees.IgnoreQueryFilters().Single(x => x.Name == "CTO"));

                //VERIFY
                EmployeeSoftDel.ShowHierarchical(ceo, x => _output.WriteLine(x), false);
                numUnSoftDeleted.ShouldEqual(4 + 3);
                var cto = context.Employees.Include(x => x.WorksFromMe).Single(x => x.Name == "CTO");
                cto.WorksFromMe.Single(x => x.SoftDeleteLevel == 0).Name.ShouldEqual("ProjectManager2");
            }
        }

        //-------------------------------------------------------------
        //disconnected state


        [Fact]
        public void TestDisconnectedResetCascadeSoftDeleteEmployeeSoftDelOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CascadeSoftDelDbContext>();
            using (var context = new CascadeSoftDelDbContext(options))
            {
                context.Database.EnsureCreated();
                var ceo = EmployeeSoftDel.SeedEmployeeSoftDel(context);

                var service = new CascadeSoftDelService(context);
                var numSoftDeleted = service.SetCascadeSoftDelete(context.Employees.Single(x => x.Name == "CTO"));
                numSoftDeleted.ShouldEqual(7+6);
            }

            using (var context = new CascadeSoftDelDbContext(options))
            {
                var service = new CascadeSoftDelService(context);

                //ATTEMPT
                var numUnSoftDeleted = service.ResetCascadeSoftDelete(context.Employees.IgnoreQueryFilters().Single(x => x.Name == "CTO"));

                //VERIFY
                numUnSoftDeleted.ShouldEqual(7 + 6);
                context.Employees.Count().ShouldEqual(11);
                context.Contracts.Count().ShouldEqual(9);
            }
        }



    }
}