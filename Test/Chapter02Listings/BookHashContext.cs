// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter02Listings
{
    public class BookHashContext : DbContext
    {
        public BookHashContext(DbContextOptions<BookHashContext> options)
            : base(options)
        {
        }

        public DbSet<BookHashReview> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<BookHashReview>()
                .HasKey(p => p.BookId);
        }
    }
}