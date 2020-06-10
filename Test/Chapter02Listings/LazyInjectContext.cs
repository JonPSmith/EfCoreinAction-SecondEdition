// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter02Listings
{
    /// <summary>
    /// This uses injection of ILazyLoader into class to do lazy loading
    /// </summary>
    public class LazyInjectContext : DbContext
    {
        public LazyInjectContext(DbContextOptions<LazyInjectContext> options)
            : base(options) { }

        public DbSet<BookLazy1> BookLazy1s { get; set; }
        public DbSet<BookLazy2> BookLazy2s { get; set; }
    }
}