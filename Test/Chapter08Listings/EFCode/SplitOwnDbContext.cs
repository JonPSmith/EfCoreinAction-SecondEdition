// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter08Listings.EFCode.Configurations;
using Test.Chapter08Listings.SplitOwnClasses;

namespace Test.Chapter08Listings.EFCode
{
    public class SplitOwnDbContext : DbContext
    {
        public SplitOwnDbContext(
            DbContextOptions<SplitOwnDbContext> options)
            : base(options)
        { }

        public DbSet<BookSummary> BookSummaries { get; set; }
        public DbSet<OrderInfo> Orders { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating
            (ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookSummaryConfig());
            modelBuilder.ApplyConfiguration(new BookDetailConfig());
            //modelBuilder.ApplyConfiguration(new OrderInfoConfig());
            modelBuilder.ApplyConfiguration(new UserConfig());
        }
    }
    /****************************************************

     * ******************************************************/
}