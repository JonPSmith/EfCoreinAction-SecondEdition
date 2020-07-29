// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter11Listings.EfClasses;

namespace Test.Chapter11Listings.EfCode
{
    public class Chapter11DbContext : DbContext
    {
        public Chapter11DbContext(
            DbContextOptions<Chapter11DbContext> options)
            : base(options)
        { }

        public DbSet<MyEntity> MyEntities { get; set; }
        public DbSet<OneEntityOptional> OneOptionalEntities { get; set; }
        public DbSet<OneEntityRequired> OneEntityRequired { get; set; }
        public DbSet<ManyEntity> ManyEntities { get; set; }

        public DbSet<NotifyEntity> Notify { get; set; }
        public DbSet<Notify2Entity> Notify2 { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotifyEntity>()
                .HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);
            modelBuilder.Entity<Notify2Entity>()
                .HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
        }
    }
}