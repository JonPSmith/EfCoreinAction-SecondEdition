// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace DataLayer.EfClasses
{
    public class BookTag
    {
        public int BookId { get; set; }

        [Required]
        [MaxLength(40)]
        public string TagId { get; set; }

        //-------------------------------------------
        //relationships

        public Book Book { get; set; }
        public Tag Tag { get; set; }
    }
}