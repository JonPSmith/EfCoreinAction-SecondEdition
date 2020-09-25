// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Chapter06Listings
{
    [Flags]
    public enum Roles {NotSet, Development = 1, Management = 2, Sales = 4, Finance = 8, HR = 16}

    public class Employee
    {
        private Employee() {} //needed for EF Core

        public Employee(string name, Roles whatTheyDo, Employee manager)
        {
            Name = name;
            WhatTheyDo = whatTheyDo;
            Manager = manager;
        }

        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public Roles WhatTheyDo { get; set; }

        //----------------------------------------
        //relationships

        public int? ManagerEmployeeId { get; set; }
        public Employee Manager { get; set; }

        public IList<Employee> WorksForMe { get; set; }
    }
}