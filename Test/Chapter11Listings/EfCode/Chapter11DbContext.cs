// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter11Listings.EfClasses;

namespace Test.Chapter11Listings.EfCode
{
    public class Chapter11DbContext : DbContext
    {
        public DbSet<MyEntity> MyEntities { get; set; }
        public DbSet<OneEntityOptional> OneOptionalEntities { get; set; }
        public DbSet<ManyEntity> ManyEntities { get; set; }

        public Chapter11DbContext(
            DbContextOptions<Chapter11DbContext> options)
            : base(options)
        { }
    }
}