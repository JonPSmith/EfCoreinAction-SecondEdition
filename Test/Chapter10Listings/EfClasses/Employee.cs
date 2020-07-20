// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter10Listings.EfClasses
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        public string Name { get; set; }

        [ConcurrencyCheck]
        public int Salary { get; set; } //#A

        public void UpdateSalary //#B
            (DbContext context, 
             int orgSalary, int newSalary)
        {
            Salary = newSalary; //#C
            context.Entry(this).Property(p => p.Salary) //#D
                .OriginalValue = orgSalary; //#D
        }

        public static void FixDeletedSalary //#E
        (DbContext context,
            Employee employee)
        {
            employee.EmployeeId = 0; //#F
            context.Add(employee); //#G
        }
    }
    /** First Listing **********************************************************
    #A The Salary property is set as a concurrency token by the attribute [ConcurrencyCheck]
    #B This method is used to update the Salary in a disconnected state
    #C I set the Salary to the new value
    #D I set the OriginalValue, which holds the data read from the database, to the original value that was shown to the user in the first part of the update
    * ********************************************************************/
    /** Second Listing **********************************************************
    #A The Salary property is set as a concurrency token by the attribute [ConcurrencyCheck]
    #B The same method used to update the Salary can be used for the update conflict, but this time it is given the original value as found when the DbUpdateConcurrencyException occured
    #C I set the Salary to the new value
    #D I set the OriginalValue, which is now the value that the database contained when the DbUpdateConcurrencyException occurred
    #E This method handles the Delete concurrency conflict. 
    #F The key must be at the CLR default value for an Add to work.
    #G I add the Employee because it was deleted from the database and therefore must be added back
    * ********************************************************************/

}