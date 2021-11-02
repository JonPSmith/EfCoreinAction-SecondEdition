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
            modelBuilder.Entity<CosmosBook>().HasNoDiscriminator();

            modelBuilder.Entity<CosmosBook>()
                .HasKey(x => x.BookId); //#C

            //net6 needed a different way to configure a collection of owned types
            //see https://docs.microsoft.com/en-gb/ef/core/modeling/owned-entities#collections-of-owned-types
            modelBuilder.Entity<CosmosBook>()
                .OwnsMany(p => p.Tags, a =>
                    {
                        a.WithOwner().HasForeignKey("BookId");
                        a.Property<int>("BookId");
                        a.HasKey(new string[] { "TagId", "BookId" });
                    });

        }
    }
    /*****************************************************************
    #A The Cosmos DB DbContext has the same structure as any other DbContext
    #B For this usage you only need read/write the CosmosBooks
    #C BookId doesn't match the By Convention rules, so you need to configure it manually
    #D The collection of CosmosTags are owned by the CosmosBook
     *****************************************************************/
}
