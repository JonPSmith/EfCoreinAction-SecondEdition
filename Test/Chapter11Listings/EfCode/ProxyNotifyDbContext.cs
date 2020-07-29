// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter11Listings.ProxyEfClasses;

namespace Test.Chapter11Listings.EfCode
{
    public class ProxyNotifyDbContext : DbContext
    {
        public ProxyNotifyDbContext(
            DbContextOptions<ProxyNotifyDbContext> options)
            : base(options)
        { }

        public DbSet<ProxyMyEntity> ProxyMyEntities { get; set; }
    }
}