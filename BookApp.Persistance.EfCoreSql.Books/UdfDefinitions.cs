// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace BookApp.Persistence.EfCoreSql.Books
{
    public static class UdfDefinitions
    {
        public static void RegisterUdfDefinitions(this ModelBuilder modelBuilder)
        {
            modelBuilder.HasDbFunction(
                () => AuthorsStringUdf(default(int)));
            modelBuilder.HasDbFunction(
                () => TagsStringUdf(default(int)));
        }

        public static string AuthorsStringUdf(int bookId)
        {
            return null;
        }

        public static string TagsStringUdf(int bookId)
        {
            return null;
        }

        public static bool FilterByTag(string tagFilter, int bookId)
        {
            return false;
        }

    }
}