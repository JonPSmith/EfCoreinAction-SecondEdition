// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter01Listings
{
    public class Chapter01DbContext : DbContext
    {
        public Chapter01DbContext(DbContextOptions<Chapter01DbContext> options)
            : base(options) { }

        public DbSet<Person> Persons { get; set; }
    }
}