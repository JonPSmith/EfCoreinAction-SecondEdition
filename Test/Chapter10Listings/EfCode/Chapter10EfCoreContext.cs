// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Test.Chapter10Listings.EfClasses;
using Order = DataLayer.EfClasses.Order;

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


        public IQueryable<TableFunctionOutput>                //#A
            GetBookTitleAndReviewsFiltered(int minReviews)    //#A
        {
            return CreateQuery(() =>                  //#B
                GetBookTitleAndReviewsFiltered(minReviews));  //#C
        }
        /************************************************************
        #A The return value, the method name, and the parameters type must match your UDF code.
        #B The FromExpression will provide the IQueryable result
        #C You place the signature of the method withing the FromExpression
         *******************************************************/

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookAuthor>()
                .HasKey(x => new { x.BookId, x.AuthorId });

            modelBuilder.HasDbFunction(
                () => MyUdfMethods.AverageVotes(default(int)));

            //modelBuilder.HasDbFunction(typeof(Chapter10EfCoreContext).GetMethod(nameof(GetBookTitleAndReviewsFiltered)));

            modelBuilder.HasDbFunction(() => GetBookTitleAndReviewsFiltered(default(int)));
        }
    }
}

