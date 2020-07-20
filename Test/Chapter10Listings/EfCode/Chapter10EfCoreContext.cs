// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter10Listings.EfCode
{
    public class Chapter10EfCoreContext : DbContext
    {
        public Chapter10EfCoreContext(                             
            DbContextOptions<Chapter10EfCoreContext> options)      
            : base(options) {}

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<PriceOffer> PriceOffers { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookAuthor>()
                .HasKey(x => new { x.BookId, x.AuthorId });

            modelBuilder.HasDbFunction(
                () => MyUdfMethods.AverageVotes(default(int)));
        }
    }
}

