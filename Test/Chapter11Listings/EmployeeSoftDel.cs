// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DataLayer.Interfaces;
using Test.Chapter08Listings.EfClasses;

namespace Test.Chapter11Listings
{
    public class EmployeeSoftDel : ICascadeSoftDelete
    {
        private EmployeeSoftDel() {}

        public EmployeeSoftDel(string name, EmployeeSoftDel manager)
        {
            Name = name;
            Manager = manager;
        }

        public int EmployeeSoftDelId { get; set; }
        public string Name { get; set; }
        public byte SoftDeleteLevel { get; set; }

        //----------------------------------------
        //relationships

        public int? ManagerEmployeeSoftDelId { get; set; }
        public EmployeeSoftDel Manager { get; set; }

        public IList<EmployeeSoftDel> WorksFromMe { get; set; }

        public override string ToString()
        {
            return $"Name: {Name} - has {WorksFromMe?.Count ?? 0} staff.";
        }

        public static void ShowHierarchical(EmployeeSoftDel employee, Action<string> output, int indent = 0, HashSet<EmployeeSoftDel> stopCircularRef = null)
        {
            stopCircularRef ??= new HashSet<EmployeeSoftDel>();
            if (stopCircularRef.Contains(employee))
            {
                output($"Circular ref back to {employee.Name}");
                return;
            }

            stopCircularRef.Add(employee);

            const int indentSize = 2;
            output(new string(' ', indent * indentSize) + employee.Name);
            foreach (var person in employee.WorksFromMe ?? new List<EmployeeSoftDel>())
            {
                ShowHierarchical(person, output, indent + 1, stopCircularRef);
            }
        }

        public static EmployeeSoftDel SeedEmployeeSoftDel(CascadeSoftDelDbContext context)
        {
            var ceo = new EmployeeSoftDel("CEO", null);
            //development
            var cto = new EmployeeSoftDel("CTO", ceo);
            var pm1 = new EmployeeSoftDel("ProjectManager1", cto);
            var dev1a = new EmployeeSoftDel("dev1a", pm1);
            var dev1b = new EmployeeSoftDel("dev1b", pm1);
            var pm2 = new EmployeeSoftDel("ProjectManager2", cto);
            var dev2a = new EmployeeSoftDel("dev2a", pm2);
            var dev2b = new EmployeeSoftDel("dev2b", pm2);
            //sales
            var salesDir = new EmployeeSoftDel("SalesDir", ceo);
            var sales1 = new EmployeeSoftDel("sales1", salesDir);
            var sales2 = new EmployeeSoftDel("sales2", salesDir);

            context.AddRange(ceo, cto, pm1, pm2, dev1a, dev1b, dev2a, dev2b, salesDir, sales1, sales2);
            context.SaveChanges();

            return ceo;
        }
    }
}