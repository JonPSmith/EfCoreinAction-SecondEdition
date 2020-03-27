// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter03Listings;

namespace Test.Chapter03Listings
{
    public class SimpleDbContext : DbContext
    {
        public DbSet<ExampleEntity> SingleEntities { get; set; }

        public SimpleDbContext(
            DbContextOptions<SimpleDbContext> options)
            : base(options)
        {}
    }
}