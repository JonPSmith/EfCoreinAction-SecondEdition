// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter11Listings.EfClasses;
using Test.Chapter11Listings.Interfaces;

namespace Test.Chapter11Listings.EfCode
{
    public class ChangeConnectDbContext : DbContext
    {
        public ChangeConnectDbContext(
            DbContextOptions<ChangeConnectDbContext> options, 
            IGetConnection getConnection)
            : base(options)
        {
            Database.SetConnectionString(
                getConnection?.CurrentConnection());
        }

        public DbSet<ConnectEntity> Entities { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

    }
}