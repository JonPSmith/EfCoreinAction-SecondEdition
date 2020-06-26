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

        public EmployeeSoftDel(string name, EmployeeSoftDel manager, EmployeeContract contact)
        {
            Name = name;
            Manager = manager;
            Contract = contact;
        }

        public int EmployeeSoftDelId { get; set; }
        public string Name { get; set; }
        public byte SoftDeleteLevel { get; set; }

        //----------------------------------------
        //relationships

        public int? ManagerEmployeeSoftDelId { get; set; }
        public EmployeeSoftDel Manager { get; set; }

        public ICollection<EmployeeSoftDel> WorksFromMe { get; set; } 

        public EmployeeContract Contract { get; set; }

        public override string ToString()
        {
            return $"Name: {Name} - has {WorksFromMe?.Count ?? 0} staff, Contract = {Contract?.ContractText ?? "-none-"} SoftDeleteLevel: {SoftDeleteLevel}";
        }


        //---------------------------------------------------
        //static unit test helper

        public static void ShowHierarchical(EmployeeSoftDel employee, Action<string> output, bool nameOnly = true, int indent = 0, HashSet<EmployeeSoftDel> stopCircularRef = null)
        {
            stopCircularRef ??= new HashSet<EmployeeSoftDel>();
            if (stopCircularRef.Contains(employee))
            {
                output($"Circular ref back to {employee.Name}");
                return;
            }

            stopCircularRef.Add(employee);

            const int indentSize = 2;
            output(new string(' ', indent * indentSize) + (nameOnly ? employee.Name : employee.ToString()));
            foreach (var person in employee.WorksFromMe ?? new List<EmployeeSoftDel>())
            {
                ShowHierarchical(person, output, nameOnly, indent + 1, stopCircularRef);
            }
        }

        public static EmployeeSoftDel SeedEmployeeSoftDel(CascadeSoftDelDbContext context)
        {
            var ceo = new EmployeeSoftDel("CEO", null, null);
            //development
            var cto = new EmployeeSoftDel("CTO", ceo, new EmployeeContract{ContractText = "$$$"});
            var pm1 = new EmployeeSoftDel("ProjectManager1", cto, new EmployeeContract { ContractText = "$$" });
            var dev1a = new EmployeeSoftDel("dev1a", pm1, new EmployeeContract { ContractText = "$" });
            var dev1b = new EmployeeSoftDel("dev1b", pm1, new EmployeeContract { ContractText = "$" });
            var pm2 = new EmployeeSoftDel("ProjectManager2", cto, new EmployeeContract { ContractText = "$$" });
            var dev2a = new EmployeeSoftDel("dev2a", pm2, null);
            var dev2b = new EmployeeSoftDel("dev2b", pm2, new EmployeeContract { ContractText = "$" });
            //sales
            var salesDir = new EmployeeSoftDel("SalesDir", ceo, new EmployeeContract { ContractText = "$$$" });
            var sales1 = new EmployeeSoftDel("sales1", salesDir, new EmployeeContract { ContractText = "$$" });
            var sales2 = new EmployeeSoftDel("sales2", salesDir, new EmployeeContract { ContractText = "$$" });

            context.AddRange(ceo, cto, pm1, pm2, dev1a, dev1b, dev2a, dev2b, salesDir, sales1, sales2);
            context.SaveChanges();

            return ceo;
        }
    }
}