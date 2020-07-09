// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter09Listings.FiveStepMigration
{
    public class App1DbContext : DbContext
    {
        public DbSet<UserPart1> Users { get; set; }

        public App1DbContext(DbContextOptions<App1DbContext> options)
            : base(options)
        { }

    }
}