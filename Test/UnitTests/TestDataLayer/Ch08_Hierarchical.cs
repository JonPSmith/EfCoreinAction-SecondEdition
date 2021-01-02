// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter08Listings.EFCode;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch08_Hierarchical
    {
        //Use this to see what happens in sql server, which does not have RESTRICT in the same way Sqlite does
        [RunnableInDebugOnly]
        public void TestDeleteManagerSqlBad()
        {
            //SETUP
            int managerId;
            var options = this.CreateUniqueClassOptions<Chapter08DbContext>();
            using var context = new Chapter08DbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var entity = new Employee
            {
                Name = "Employee1",
                Manager = new Employee { Name = "Employee2" }
            };
            context.Add(entity);
            context.SaveChanges();
            managerId = (int)entity.Manager.EmployeeId;

            context.ChangeTracker.Clear();

            var manager = context.Employees.Find(managerId);
            context.Remove(manager);
            var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

            //VERIFY
            ex.InnerException.Message.ShouldContain(
                "The DELETE statement conflicted with the SAME TABLE REFERENCE constraint \"FK_Employees_Employees_ManagerEmployeeId\".");
        }

        [Fact]
        public void TestDeleteManagerClientSetNullOnDeleteOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            int managerId;
            using var context = new Chapter08DbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var entity = new EmployeeShortFk
            {
                Name = "Employee1",
                Manager = new EmployeeShortFk { Name = "Employee2" }
            };
            context.Add(entity);
            context.SaveChanges();
            managerId = (int) entity.Manager.EmployeeShortFkId;

            context.ChangeTracker.Clear();

            var manager = context.EmployeeShortFks.Find(managerId);
            context.Remove(manager);
            var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

            //VERIFY
            ex.InnerException.Message.ShouldEqual(
                "SQLite Error 19: 'FOREIGN KEY constraint failed'.");
        }

        [Fact]
        public void TestDeleteManagerDefaultOnDeleteOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            int managerId;
            using var context = new Chapter08DbContext(options);
            context.Database.EnsureCreated();

            var entity = new Employee
            {
                Name = "Employee1",
                Manager = new Employee {Name = "Employee2"}
            };
            context.Add(entity);
            context.SaveChanges();
            managerId = (int)entity.Manager.EmployeeId;

            context.ChangeTracker.Clear();

            //ATTEMPT
            var manager = context.Employees.Find(managerId);
            context.Remove(manager);
            var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

            //VERIFY
            ex.InnerException.Message.ShouldEqual(
                "SQLite Error 19: 'FOREIGN KEY constraint failed'.");
        }

        [Fact]
        public void TestEmployeeAndManagerCreateOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using var context = new Chapter08DbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var entity = new Employee
            {
                Name = "Employee1",
                Manager = new Employee {  Name = "Employee2"}
            };
            context.Add(entity);
            context.SaveChanges();

            //VERIFY
            context.Employees.Count().ShouldEqual(2);
            var employees = context.Employees.Include(x => x.Manager).OrderBy(x => x.Name).ToList();
            employees[0].Manager.ShouldEqual(employees[1]);
            employees[1].Manager.ShouldBeNull();
        }

        [Fact]
        public void TestEmployeeCreateOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using var context = new Chapter08DbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var entity = new Employee
            {
                Name = "Employee1"
            };
            context.Add(entity);
            context.SaveChanges();

            //VERIFY
            context.Employees.Count().ShouldEqual(1);
            context.Employees.First().EmployeeId.ShouldEqual(1);
            context.Employees.First().ManagerEmployeeId.ShouldBeNull();
        }
    }
}