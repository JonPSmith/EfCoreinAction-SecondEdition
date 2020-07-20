// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Test.Chapter10Listings.EfClasses;
using Test.Chapter10Listings.EfCode;
using TestSupport.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch10_ConcurrencyDisconnect
    {
        private readonly DbContextOptions<ConcurrencyDbContext> _options;

        public Ch10_ConcurrencyDisconnect()
        {
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder =
                new DbContextOptionsBuilder<ConcurrencyDbContext>();
            optionsBuilder.UseSqlServer(connection);
            _options = optionsBuilder.Options;
            using (var context = new ConcurrencyDbContext(_options))
            {
                context.Database.EnsureCreated();

                var johnDoe = GetJohnDoeRecord(context);
                if (johnDoe == null)
                    context.Add(new Employee { Name = "John Doe", Salary = 1000});
                else
                {
                    johnDoe.Salary = 1000;
                }
                context.SaveChanges();
            }
        }


        private static Employee GetJohnDoeRecord(ConcurrencyDbContext context)
        {
            return context.Employees.SingleOrDefault(p => p.Name == "John Doe");
        }

        [Fact]
        public void TestDisconnectedUpdateOk()
        {
            //SETUP
            Employee entity;
            using (var context = new ConcurrencyDbContext(_options))
            {
                entity = GetJohnDoeRecord(context);
            }

            using (var context = new ConcurrencyDbContext(_options))
            {
                //ATTEMPT
                context.Update(entity);
                entity.UpdateSalary(context, 1000, 1100);
                context.SaveChanges();

                //VERIFY
                GetJohnDoeRecord(context).Salary.ShouldEqual(1100);
            }
        }

        [Fact]
        public void TestDisconnectedUpdateThrowExceptionOk()
        {
            //SETUP
            int johnDoeId;
            using (var context = new ConcurrencyDbContext(_options))
            {
                johnDoeId = GetJohnDoeRecord(context).EmployeeId;
            }

            using (var contextBoss = new ConcurrencyDbContext(_options))
            using (var contextHr = new ConcurrencyDbContext(_options))
            {
                //ATTEMPT
                var jdBoss = contextBoss.Employees.Find(johnDoeId);
                var jdHr = contextHr.Employees.Find(johnDoeId);
                jdBoss.UpdateSalary(contextBoss, 1000, 1100);
                contextBoss.SaveChanges();
                jdHr.UpdateSalary(contextHr,1000,1025);

                var ex = Assert.Throws<DbUpdateConcurrencyException>(() => contextHr.SaveChanges());

                //VERIFY
                ex.Message.StartsWith("Database operation expected to affect 1 row(s) but actually affected 0 row(s). Data may have been modified or deleted since entities were loaded. ")
                    .ShouldBeTrue();
            }
        }

        [Fact]
        public void TestDisconnectedDeleteThrowExceptionOk()
        {
            //SETUP
            int johnDoeId;
            using (var context = new ConcurrencyDbContext(_options))
            {
                johnDoeId = GetJohnDoeRecord(context).EmployeeId;
            }

            using (var context = new ConcurrencyDbContext(_options))
            {
                //ATTEMPT
                var entity = context.Employees.Find(johnDoeId);
                context.Database.ExecuteSqlRaw(
                    "DELETE dbo.Employees WHERE EmployeeId = @p0",
                    johnDoeId);
                entity.UpdateSalary(context, 1000, 1100);

                var ex = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());

                //VERIFY
                ex.Message.StartsWith("Database operation expected to affect 1 row(s) but actually affected 0 row(s). Data may have been modified or deleted since entities were loaded. ")
                    .ShouldBeTrue();
            }
        }

        [Fact]
        public void TestDisconnectedUpdateDiagnoseOk()
        {
            //SETUP
            int johnDoeId;
            using (var context = new ConcurrencyDbContext(_options))
            {
                johnDoeId = GetJohnDoeRecord(context).EmployeeId;
            }

            using (var contextBoss = new ConcurrencyDbContext(_options))
            using (var contextHr = new ConcurrencyDbContext(_options))
            {
                //ATTEMPT
                var jdBoss = contextBoss.Employees.Find(johnDoeId);
                var jdHr = contextHr.Employees.Find(johnDoeId);
                jdBoss.UpdateSalary(contextBoss, 1000, 1100);
                contextBoss.SaveChanges();
                jdHr.UpdateSalary(contextHr, 1000, 1025);

                string message = null;
                try
                {
                    contextHr.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    message = DiagnoseSalaryConflict(
                        contextHr, entry);
                }

                //VERIFY
                message.ShouldEqual("The Employee John Doe's salary was set to 1100 by another user. Click Update to use your new salary of 1025 or Cancel to leave the salary at 1100.");
            }
        }

        [Fact]
        public void TestDisconnectedDeleteDiagnoseOk()
        {
            //SETUP
            int employeeId;
            using (var context = new ConcurrencyDbContext(_options))
            {
                employeeId = GetJohnDoeRecord(context).EmployeeId;
            }

            using (var context = new ConcurrencyDbContext(_options))
            {
                //ATTEMPT
                var entity = context //#A
                    .Find<Employee>(employeeId); //#A
                context.Database.ExecuteSqlRaw(
                    "DELETE dbo.Employees WHERE EmployeeId = @p0",
                    employeeId);
                entity.UpdateSalary(context, 1000, 1100);

                string message = null;
                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    message = DiagnoseSalaryConflict(
                        context, entry);
                }
                /***********************************************************************
                #A The 
                 * ********************************************************************/

                //VERIFY
                message.ShouldEqual("The Employee John Doe was deleted by another user. Click Add button to add back with salary of 1100 or Cancel to leave deleted.");
            }
        }

        private string DiagnoseSalaryConflict( //#A
            ConcurrencyDbContext context, 
            EntityEntry entry)
        {
            var employee = entry.Entity
                as Employee;
            if (employee == null) //#B
                throw new NotSupportedException(
        "Don't know how to handle concurrency conflicts for " +
                    entry.Metadata.Name);

            var databaseEntity = //#C
                context.Employees.AsNoTracking() //#D
                    .SingleOrDefault(p => 
                        p.EmployeeId == employee.EmployeeId);

            if (databaseEntity == null) //#E
                return
        $"The Employee {employee.Name} was deleted by another user. " +
        $"Click Add button to add back with salary of {employee.Salary}" +
        " or Cancel to leave deleted."; //#F

            return //#G
        $"The Employee {employee.Name}'s salary was set to " +
        $"{databaseEntity.Salary} by another user. " +
        $"Click Update to use your new salary of {employee.Salary}" +
        $" or Cancel to leave the salary at {databaseEntity.Salary}."; //#G
        }
        /*********************************************************************
        #A This is called if there is a DbUpdateConcurrencyException. Its job is not to fix the problem, but form a useful message to show the user
        #B If the enity that failed wasn't an Employee then I throw an exception, as this code cannot handle that.
        #C I want to get the data that someone else wrote into the database after my read. 
        #D This entity MUST be read as NoTracking otherwise it will interfere with the same entity we are trying to write
        #E I check whether this was a delete conflict, that is, the employee was deleted since the user attempted to update it
        #F This is the error message to display to the user, with their two choices on how to carry on
        #G Else it must be an update conflict, so I return a different error message with the two choices for this case.
         * ******************************************************************/

        [Fact]
        public void TestDisconnectedUpdateFixUpdateOk()
        {
            //SETUP
            int johnDoeId;
            using (var context = new ConcurrencyDbContext(_options))
            {
                johnDoeId = GetJohnDoeRecord(context).EmployeeId;
            }

            int orgSalary = 0;
            using (var contextBoss = new ConcurrencyDbContext(_options))
            using (var contextHr = new ConcurrencyDbContext(_options))
            {
                //ATTEMPT
                var jdBoss = contextBoss.Employees.Find(johnDoeId);
                var jdHr = contextHr.Employees.Find(johnDoeId);
                jdBoss.UpdateSalary(contextBoss, 1000, 1100);
                contextBoss.SaveChanges();
                jdHr.UpdateSalary(contextHr, 1000, 1025);

                try
                {
                    contextHr.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    orgSalary = contextHr.Employees.AsNoTracking().Single(x => x.EmployeeId == johnDoeId).Salary;
                }
            }

            using (var context = new ConcurrencyDbContext(_options))
            {
                //VERIFY
                var johnDoe = context.Employees.Find(johnDoeId);
                johnDoe.UpdateSalary(context, orgSalary, 1025);
                context.SaveChanges();
            }

            using (var context = new ConcurrencyDbContext(_options))
            {
                //VERIFY
                var johnDoe = context.Employees.Find(johnDoeId);
                johnDoe.Salary.ShouldEqual(1025);
            }
        }

        [Fact]
        public void TestDisconnectedFixDeleteOk()
        {
            //SETUP
            int johnDoeId;
            using (var context = new ConcurrencyDbContext(_options))
            {
                johnDoeId = GetJohnDoeRecord(context).EmployeeId;
            }

            Employee disconnected = null;
            using (var context = new ConcurrencyDbContext(_options))
            {
                //ATTEMPT
                var johnDoe = context.Employees.Find(johnDoeId);
                context.Database.ExecuteSqlRaw(
                    "DELETE dbo.Employees WHERE EmployeeId = @p0",
                    johnDoeId);
                johnDoe.UpdateSalary(context, 1000, 1100);

                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    disconnected = johnDoe;
                }
            }

            using (var context = new ConcurrencyDbContext(_options))
            {
                Employee.FixDeletedSalary(context, disconnected);
                context.SaveChanges();
            }

            using (var context = new ConcurrencyDbContext(_options))
            {
                //VERIFY
                var johnDoe = GetJohnDoeRecord(context);
                johnDoe.Salary.ShouldEqual(1100);
            }
        }
    }
}