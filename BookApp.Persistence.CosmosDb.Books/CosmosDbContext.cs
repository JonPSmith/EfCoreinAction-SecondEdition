using System;
using BookApp.Domain.Books;
using Microsoft.EntityFrameworkCore;

namespace BookApp.Persistence.CosmosDb.Books
{
    public class CosmosDbContext : DbContext
    {
        public CosmosDbContext(
            DbContextOptions<CosmosDbContext> options) 
            : base(options)  
        { }

        public DbSet<CosmosBook> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CosmosBook>().HasKey(x => x.BookId);

            modelBuilder.Entity<CosmosBook>()
                .OwnsMany(p => p.Tags);
        }
    }
}
