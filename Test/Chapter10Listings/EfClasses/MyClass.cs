// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Chapter10Listings.EfClasses
{
    public class MyClass
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid MyClassId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SecondaryKey { get; set; }
    }
}