// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter11Listings
{
    public class SoftDelDbContext : DbContext
    {
        public SoftDelDbContext(DbContextOptions<SoftDelDbContext> options)
            : base(options) { }

        public DbSet<EmployeeSoftCascade> Employees { get; set; }
        public DbSet<EmployeeContract> Contracts { get; set; }

        public DbSet<BookSoftDel> Books { get; set; }

        public DbSet<CompanySoftCascade> Companies { get; set; }
        public DbSet<QuoteSoftCascade> Quotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeSoftCascade>()
                .HasMany(x => x.WorksFromMe)
                .WithOne(x => x.Manager)
                .HasForeignKey(x => x.ManagerEmployeeSoftDelId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<EmployeeSoftCascade>()
                .HasOne(x => x.Contract)
                .WithOne()
                .HasForeignKey<EmployeeContract>(x => x.EmployeeSoftCascadeId)
                .OnDelete(DeleteBehavior.ClientCascade);


            modelBuilder.Entity<EmployeeSoftCascade>()
                .HasQueryFilter(x => x.SoftDeleteLevel == 0);
            modelBuilder.Entity<EmployeeContract>()
                .HasQueryFilter(x => x.SoftDeleteLevel == 0);

            modelBuilder.Entity<BookSoftDel>()
                .HasQueryFilter(x => !x.SoftDeleted);

            modelBuilder.Entity<CompanySoftCascade>()
                .HasQueryFilter(x => x.SoftDeleteLevel == 0);
            modelBuilder.Entity<QuoteSoftCascade>()
                .HasQueryFilter(x => x.SoftDeleteLevel == 0);
        }
    }
}