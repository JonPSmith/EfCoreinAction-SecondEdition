// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Domain.Books.SupportTypes;
using Microsoft.EntityFrameworkCore;

namespace BookApp.Persistence.EfCoreSql.Books
{
    public static class BookDetectChangesExtensions
    {
        public static void ChangeChecker(this DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries()
                .Where(e =>                          
                     e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                var tracked = entry.Entity as ICreatedUpdated;         //#D
                tracked?.LogAddUpdate(entry.State == EntityState.Added);
                if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(ICreatedUpdated.LastUpdatedUtc))  
                        .IsModified = true;
                }
            }
        }
    }
}