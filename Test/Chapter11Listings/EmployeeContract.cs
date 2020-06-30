// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using DataLayer.Interfaces;

namespace Test.Chapter11Listings
{
    public class EmployeeContract : ICascadeSoftDelete
    {
        [Key]
        public int EmployeeSoftCascadeId { get; set; }

        public string ContractText { get; set; }

        public byte SoftDeleteLevel { get; set; }
    }
}