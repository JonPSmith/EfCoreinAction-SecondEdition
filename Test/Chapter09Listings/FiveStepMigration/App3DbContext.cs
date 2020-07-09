// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter09Listings.FiveStepMigration
{
    public class App3DbContext : DbContext
    {
        public DbSet<UserPart5> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public App3DbContext(DbContextOptions<App3DbContext> options)
            : base(options)
        { }

    }
}