using System;
using BookApp.Domain.Books;
using Microsoft.EntityFrameworkCore;

namespace BookApp.Persistence.CosmosDb.Books
{
    public class CosmosDbContext : DbContext //#A
    {
        public CosmosDbContext(
            DbContextOptions<CosmosDbContext> options) 
            : base(options)  
        { }

        public DbSet<CosmosBook> Books { get; set; } //#B

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CosmosBook>()
                .HasKey(x => x.BookId); //#C

            modelBuilder.Entity<CosmosBook>() //#D
                .OwnsMany(p => p.Tags);       //#D
        }
    }
    /*****************************************************************
    #A The Cosmos DB DbContext has the same structure as any other DbContext
    #B For this usage you only need read/write the CosmosBooks
    #C BookId doesn't match the By Convention rules, so you need to configure it manually
    #D The collection of CosmosTags are owned by the CosmosBook
     *****************************************************************/
}
