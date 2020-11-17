// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter16Listings
{
    public class Ch16CosmosDbContext : DbContext
    {
        public Ch16CosmosDbContext(
            DbContextOptions<Ch16CosmosDbContext> options)
            : base(options)
        { }

        public DbSet<CosmosGuidKey> GuidKeyItems { get; set; } //#B
        public DbSet<CosmosCompositeKey> ComKeyItems { get; set; } //#B

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CosmosCompositeKey>()
                .HasKey(x => new {x.Key1, x.Key2});
        }
    }
}