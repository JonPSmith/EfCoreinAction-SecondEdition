// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter06Listings;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch06_HierarchicalData
    {
        private readonly ITestOutputHelper _output;

        public Ch06_HierarchicalData(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestAddTestEmployeesToDb()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            using var context = new Chapter06Context(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            context.AddTestEmployeesToDb();

            //VERIFY
            context.Employees.Count().ShouldEqual(11);
        }

        [Fact]
        public void TestThenIncludeWorksForMe()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            using var context = new Chapter06Context(options);
            context.Database.EnsureCreated();
            context.AddTestEmployeesToDb();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var all = context.Employees
                .Include(x => x.WorksForMe)
                .ThenInclude(x => x.WorksForMe)
                .ToList();

            //VERIFY
            all.Count.ShouldEqual(11);
            all.Count(x => x.Manager != null).ShouldEqual(10);
            all.Count(x => x.WorksForMe.Any()).ShouldEqual(5);
            var top = all.Single(x => x.Manager == null);
            top.ShowHierarchical(s => _output.WriteLine(s));
        }

        [Fact]
        public void TestLoadWorksForMeNoWorkingAsNoTracking()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            using var context = new Chapter06Context(options);
            context.Database.EnsureCreated();
            context.AddTestEmployeesToDb();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var all = context.Employees
                .AsNoTracking()
                .Include(x => x.WorksForMe)
                .ToList();

            //VERIFY
            all.Count.ShouldEqual(11);
            all.Count(x => x.Manager == null).ShouldEqual(11);
            all.Count(x => x.WorksForMe.Any()).ShouldEqual(5);
        }

        [Fact]
        public void TestLoadWorksForMeTracked()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            using var context = new Chapter06Context(options);
            context.Database.EnsureCreated();
            context.AddTestEmployeesToDb();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var all = context.Employees.Include(x => x.WorksForMe)
                .ToList();

            //VERIFY
            all.Count.ShouldEqual(11);
            all.Count(x => x.Manager != null).ShouldEqual(10);
            all.Count(x => x.WorksForMe.Any()).ShouldEqual(5);
            var top = all.Single(x => x.Manager == null);
            top.ShowHierarchical(s => _output.WriteLine(s));
        }


        [Fact]
        public void TestLoadWorksForMeAsNoTrackingWithIdentityResolution()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            using var context = new Chapter06Context(options);
            context.Database.EnsureCreated();
            context.AddTestEmployeesToDb();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var all = context.Employees
                .AsNoTrackingWithIdentityResolution()
                .Include(x => x.WorksForMe)
                .ToList();

            //VERIFY
            all.Count.ShouldEqual(11);
            all.Count(x => x.Manager != null).ShouldEqual(10);
            all.Count(x => x.WorksForMe.Any()).ShouldEqual(5);
            var top = all.Single(x => x.Manager == null);
            top.ShowHierarchical(s => _output.WriteLine(s));
        }

        [Fact]
        public void TestLoadManager()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            using var context = new Chapter06Context(options);
            context.Database.EnsureCreated();
            context.AddTestEmployeesToDb();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var all = context.Employees.Include(x => x.Manager)
                .ToList();

            //VERIFY
            all.Count.ShouldEqual(11);
            all.Count(x => x.Manager != null).ShouldEqual(10);
            all.Count(x => x.WorksForMe != null).ShouldEqual(5);
            all.Count(x => x.WorksForMe == null).ShouldEqual(6);
        }

        [Fact]
        public void TestLoadByRoleDbShowQuery()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            using var context = new Chapter06Context(options);
            context.Database.EnsureCreated();
            context.AddTestEmployeesToDb();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var query = context.Employees
                .Include(x => x.WorksForMe)
                .Where(x => x.WhatTheyDo.HasFlag(Roles.Development));
            var devDept = query.ToList();

            //VERIFY
            _output.WriteLine(query.ToQueryString());
            devDept.Count.ShouldEqual(7);
            var cto = devDept.Single(x => x.Manager == null);
            cto.ShowHierarchical(s => _output.WriteLine(s));
        }

        [Fact]
        public void TestLoadByRoleDb()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            using var context = new Chapter06Context(options);
            context.Database.EnsureCreated();
            context.AddTestEmployeesToDb();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var devDept = context.Employees                          //#A
                .Include(x => x.WorksForMe)                         //#B
                .Where(x => x.WhatTheyDo.HasFlag(Roles.Development)) //#C
                .ToList();
            /********************************************************
                #A The database holds all the Employees
                #B One Include is all that you need - relational fixup will work out what is linked to what
                #C This filters the employees down to ones that work in Development
                 **********************************************/

            //VERIFY
            devDept.Count.ShouldEqual(7);
        }



    }
}