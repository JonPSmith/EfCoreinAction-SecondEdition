// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter11Listings
{
    public class CascadeSoftDelDbContext : DbContext
    {
        public CascadeSoftDelDbContext(DbContextOptions<CascadeSoftDelDbContext> options)
            : base(options) { }

        public DbSet<EmployeeSoftDel> Employees { get; set; }
        public DbSet<EmployeeContract> Contracts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeSoftDel>()
                .HasMany(x => x.WorksFromMe)
                .WithOne(x => x.Manager)
                .HasForeignKey(x => x.ManagerEmployeeSoftDelId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<EmployeeSoftDel>()
                .HasOne(x => x.Contract)
                .WithOne()
                .HasForeignKey<EmployeeContract>(x => x.EmployeeSoftDelId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<EmployeeSoftDel>()
                .HasQueryFilter(x => x.SoftDeleteLevel == 0);
            modelBuilder.Entity<EmployeeContract>()
                .HasQueryFilter(x => x.SoftDeleteLevel == 0);
        }
    }
}