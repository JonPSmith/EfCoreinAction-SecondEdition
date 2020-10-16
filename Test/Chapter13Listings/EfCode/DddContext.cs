// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter13Listings.EfClasses;

namespace Test.Chapter13Listings.EfCode
{
    public class DddContext : DbContext
    {
        public DddContext(DbContextOptions<DddContext> options)
            : base(options)
        { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookAuthor>().HasKey(p => new { p.BookId, p.AuthorId });
        }
    }
}