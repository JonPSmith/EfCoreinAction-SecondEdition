// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Chapter08Listings.EfClasses
{
    public class EmployeeShortFk
    {
        public int EmployeeShortFkId { get; set; }

        public string Name { get; set; }

        //------------------------------
        //Relationships

        public int? ManagerId { get; set; }

        [ForeignKey(nameof(ManagerId))]      //#A
        public EmployeeShortFk Manager { get; set; }
    }
    /************************************************
    #A This Data Annotation defines which property is the Foreign key for the 'Manager' navigational property
     * ***********************************************/
}