// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter02Listings
{
    /// <summary>
    /// This uses Microsoft.EntityFrameworkCore.Proxies and virtual to do lazy loading
    /// </summary>
    public class LazyProxyContext : DbContext
    {
        public LazyProxyContext(DbContextOptions<LazyProxyContext> options)
            : base(options) { }

        public DbSet<BookLazyProxy> Books { get; set; }
        
    }
}