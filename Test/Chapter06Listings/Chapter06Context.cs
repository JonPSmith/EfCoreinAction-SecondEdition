// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter06Listings
{
    public class Chapter06Context : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<ManyTop> ManyTops { get; set; }

        public DbSet<BookNotSafe> Books { get; set; }

        public DbSet<OnePrincipal> OnePrincipals { get; set; }
        public DbSet<OneDependent> OneDependents { get; set; }

        public Chapter06Context(DbContextOptions<Chapter06Context> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasMany(x => x.WorksForMe)
                .WithOne(x => x.Manager)
                .HasForeignKey(x => x.ManagerEmployeeId);
        }
    }
}