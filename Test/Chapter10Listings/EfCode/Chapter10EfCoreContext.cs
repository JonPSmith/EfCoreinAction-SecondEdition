// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using DataLayer.EfCode.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter10Listings.EfCode
{
    public class Chapter10EfCoreContext : DbContext
    {
        public DbSet<Book> Books { get; set; }              
        public DbSet<Author> Authors { get; set; }          
        public DbSet<PriceOffer> PriceOffers { get; set; }  
        public DbSet<Order> Orders { get; set; }            

        public Chapter10EfCoreContext(                             
            DbContextOptions<Chapter10EfCoreContext> options)      
            : base(options) {}

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookAuthor>()
                .HasKey(x => new { x.BookId, x.AuthorId });

            //needed this to add .HasSchema - see bug https://github.com/aspnet/EntityFrameworkCore/issues/9663
            modelBuilder.HasDbFunction(
                () => MyUdfMethods.AverageVotes(default(int)))
                .HasSchema("dbo");
        }

    }
}

