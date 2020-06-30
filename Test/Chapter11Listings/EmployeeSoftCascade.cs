// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using DataLayer.Interfaces;

namespace Test.Chapter11Listings
{
    public class EmployeeSoftCascade : ICascadeSoftDelete
    {
        private EmployeeSoftCascade() {}

        public EmployeeSoftCascade(string name, EmployeeSoftCascade manager, EmployeeContract contact)
        {
            Name = name;
            Manager = manager;
            Contract = contact;
        }

        public int EmployeeSoftCascadeId { get; set; }
        public string Name { get; set; }
        public byte SoftDeleteLevel { get; set; }

        //----------------------------------------
        //relationships

        public int? ManagerEmployeeSoftDelId { get; set; }
        public EmployeeSoftCascade Manager { get; set; }

        public ICollection<EmployeeSoftCascade> WorksFromMe { get; set; } 

        public EmployeeContract Contract { get; set; }

        public override string ToString()
        {
            return $"Name: {Name} - has {WorksFromMe?.Count ?? 0} staff, Contract = {Contract?.ContractText ?? "-none-"} SoftDeleteLevel: {SoftDeleteLevel}";
        }


        //---------------------------------------------------
        //static unit test helper

        public static void ShowHierarchical(EmployeeSoftCascade employee, Action<string> output, bool nameOnly = true, int indent = 0, HashSet<EmployeeSoftCascade> stopCircularRef = null)
        {
            stopCircularRef ??= new HashSet<EmployeeSoftCascade>();
            if (stopCircularRef.Contains(employee))
            {
                output($"Circular ref back to {employee.Name}");
                return;
            }

            stopCircularRef.Add(employee);

            const int indentSize = 2;
            output(new string(' ', indent * indentSize) + (nameOnly ? employee.Name : employee.ToString()));
            foreach (var person in employee.WorksFromMe ?? new List<EmployeeSoftCascade>())
            {
                ShowHierarchical(person, output, nameOnly, indent + 1, stopCircularRef);
            }
        }

        public static EmployeeSoftCascade SeedEmployeeSoftDel(SoftDelDbContext context)
        {
            var ceo = new EmployeeSoftCascade("CEO", null, null);
            //development
            var cto = new EmployeeSoftCascade("CTO", ceo, new EmployeeContract{ContractText = "$$$"});
            var pm1 = new EmployeeSoftCascade("ProjectManager1", cto, new EmployeeContract { ContractText = "$$" });
            var dev1a = new EmployeeSoftCascade("dev1a", pm1, new EmployeeContract { ContractText = "$" });
            var dev1b = new EmployeeSoftCascade("dev1b", pm1, new EmployeeContract { ContractText = "$" });
            var pm2 = new EmployeeSoftCascade("ProjectManager2", cto, new EmployeeContract { ContractText = "$$" });
            var dev2a = new EmployeeSoftCascade("dev2a", pm2, null);
            var dev2b = new EmployeeSoftCascade("dev2b", pm2, new EmployeeContract { ContractText = "$" });
            //sales
            var salesDir = new EmployeeSoftCascade("SalesDir", ceo, new EmployeeContract { ContractText = "$$$" });
            var sales1 = new EmployeeSoftCascade("sales1", salesDir, new EmployeeContract { ContractText = "$$" });
            var sales2 = new EmployeeSoftCascade("sales2", salesDir, new EmployeeContract { ContractText = "$$" });

            context.AddRange(ceo, cto, pm1, pm2, dev1a, dev1b, dev2a, dev2b, salesDir, sales1, sales2);
            context.SaveChanges();

            return ceo;
        }
    }
}