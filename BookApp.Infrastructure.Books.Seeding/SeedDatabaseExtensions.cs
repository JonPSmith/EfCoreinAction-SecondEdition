// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Persistence.EfCoreSql.Books;

namespace BookApp.Infrastructure.Books.Seeding
{
    public static class SeedDatabaseExtensions
    {
        private const string SummaryBookSearchName = "ManningBooks*.json";
        private const string DetailBookSearchName = "ManningDetails*.json";
        public const string SeedFileSubDirectory = "seedData";

        public static async Task SeedDatabaseIfNoBooksAsync(this BookDbContext context, string wwwRootDir)
        {
            var seedDirPath = Path.Combine(wwwRootDir, SeedFileSubDirectory);
            var books = seedDirPath.LoadBooks(SummaryBookSearchName, DetailBookSearchName).ToList();
            context.AddRange(books);
            await context.SaveChangesAsync();
        }
    }
}