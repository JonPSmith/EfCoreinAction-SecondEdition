// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Domain.Books;
using BookApp.Persistence.EfCoreSql.Books;

namespace BookApp.UI.Models
{
    public class DatabaseStatsDto
    {
        public int NumBooks { get; }
        public int NumReviews { get; }
        public int NumAuthors { get; }
        public int NumTags { get; }

        public DatabaseStatsDto(BookDbContext context)
        {
            NumBooks = context.Books.Count();
            NumReviews = context.Set<Review>().Count();
            NumAuthors = context.Authors.Count();
            NumTags = context.Tags.Count();
        }
    }
}