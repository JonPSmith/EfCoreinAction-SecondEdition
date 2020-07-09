// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter09Listings.FiveStepMigration
{
    [Keyless]
    public class ReadOnlyUserWithAddress
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Street { get; set; }
        public string City { get; set; }
    }
}