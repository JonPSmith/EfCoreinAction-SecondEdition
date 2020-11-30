// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter14Listings
{
    public class Chapter14DbContext : DbContext
    {
        public Chapter14DbContext(
            DbContextOptions<Chapter14DbContext> options)
        : base(options)
        { }

        public DbSet<MyEntity> MyEntities { get; set; }
    }
}