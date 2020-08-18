// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using BookApp.Persistence.EfCoreSql.Books;

namespace BookApp.Infrastructure.Books.Seeding
{
    public static class SeedDatabaseExtensions
    {
        public static async Task SeedDatabaseIfNoBooksAsync(this BookDbContext context, string wwwRootDir)
        {
            var books = wwwRootDir.LoadBooks("ManningBooks*.json").ToList();
            context.AddRange(books);
            await context.SaveChangesAsync();
        }
    }
}