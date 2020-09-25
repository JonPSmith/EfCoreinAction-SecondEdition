// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace Test.Chapter06Listings
{
    public static class EmployeeExtensions
    {
        public static void AddTestEmployeesToDb(this Chapter06Context context)
        {
            var ceo = new Employee("CEO", Roles.Management, null);
            //development
            var cto = new Employee("CTO", Roles.Management | Roles.Development, ceo);
            var pm1 = new Employee("ProjectManager1", Roles.Management | Roles.Development, cto);
            var dev1a = new Employee("dev1a", Roles.Development, pm1);
            var dev1b = new Employee("dev1b",  Roles.Development, pm1);
            var pm2 = new Employee("ProjectManager2", Roles.Management | Roles.Development, cto);
            var dev2a = new Employee("dev2a", Roles.Development, pm2);
            var dev2b = new Employee("dev2b", Roles.Development, pm2);
            //sales
            var salesDir = new Employee("SalesDir", Roles.Management | Roles.Sales, ceo);
            var sales1 = new Employee("sales1",  Roles.Sales, salesDir);
            var sales2 = new Employee("sales2", Roles.Sales, salesDir);

            context.AddRange(ceo, cto,pm1, pm2, dev1a, dev1b, dev2a, dev2b, salesDir, sales1, sales2);
            context.SaveChanges();
        }

        public static void ShowHierarchical(this Employee employee, Action<string> output, int indent = 0)
        {
            const int indentSize = 2;
            output(new string(' ', indent * indentSize) + employee.Name);
            foreach (var person in employee.WorksForMe)
            {
                person.ShowHierarchical(output, indent + 1);
            }
        }
    }
}