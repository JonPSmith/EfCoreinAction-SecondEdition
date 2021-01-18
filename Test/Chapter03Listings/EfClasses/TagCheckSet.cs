// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Test.Chapter03Listings.EfClasses
{
    public class TagCheckSet
    {
        [Key]
        [Required]
        [MaxLength(40)]
        public string TagId { get; set; }

        public ICollection<BookCheckSet> Books { get; set; }
    }
}