// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Test.Chapter11Listings.EfClasses;

namespace Test.Chapter11Listings.EfCode
{
    public class SqlQueryDbContext : DbContext
    {
        public SqlQueryDbContext(DbContextOptions<SqlQueryDbContext> options) 
            : base(options) { }

        //#B
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<PriceOffer> PriceOffers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        
        public DbSet<BookSqlQuery> BookSqlQueries { get; set; }  //#A


        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<BookAuthor>()
                .HasKey(x => new { x.BookId, x.AuthorId });

            modelBuilder.Entity<Book>() 
                .HasQueryFilter(p => !p.SoftDeleted);

            modelBuilder.Entity<BookSqlQuery>().ToSqlQuery(  //#B
        @"SELECT BookId                                   
             ,Title                                       
             ,(SELECT AVG(CAST([r0].[NumStars] AS float)) 
        FROM Review AS r0                              
        WHERE t.BookId = r0.BookId)  AS AverageVotes  
        FROM Books AS t");                               //#C
        }
    }
}