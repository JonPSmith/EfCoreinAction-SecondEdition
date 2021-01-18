// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter03Listings.EfClasses;

namespace Test.Chapter03Listings.EfCode
{
    public class Chapter3DbContext : DbContext
    {
        public DbSet<BookCheckSet> BookCheckSets { get; set; }

        public DbSet<ExampleEntity> SingleEntities { get; set; }

        public Chapter3DbContext(
            DbContextOptions<Chapter3DbContext> options)
            : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder) //#E
        {
            modelBuilder.Entity<BookAuthorCheckSet>()
                .HasKey(x => new { x.BookId, x.AuthorId });
        }
    }
}