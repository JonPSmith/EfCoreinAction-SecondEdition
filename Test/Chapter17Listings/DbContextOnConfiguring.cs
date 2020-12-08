// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter17Listings
{
    public class DbContextOnConfiguring : DbContext
    {
        private const string ConnectionString
            = "Server=(localdb)\\mssqllocaldb;Database=EfCore.TestSupport-Test-OnConfiguring;Trusted_Connection=True";

        public DbContextOnConfiguring(               //#B
            DbContextOptions<DbContextOnConfiguring> //#B
                options)                                 //#B
            : base(options) { } //#B

        public DbContextOnConfiguring() { } //#C


        public DbSet<MyEntity> MyEntities { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)    //#A
            {
                optionsBuilder
                    .UseSqlServer(ConnectionString);
            }
        }
    }
}