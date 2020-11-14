// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Domain.Books;

namespace BookApp.Infrastructure.Books.CosmosDb.Services
{
    public static class SqlBookToCosmosBookExt
    {
        public static IQueryable<CosmosBook> MapBookToCosmosBook(this IQueryable<Book> books)
        {
            return books
                .Select(p => new CosmosBook
                {
                    BookId = p.BookId,
                    Title = p.Title,
                    PublishedOn = p.PublishedOn,
                    EstimatedDate = p.EstimatedDate,
                    YearPublished = p.PublishedOn.Year,
                    OrgPrice = p.OrgPrice,
                    ActualPrice = p.ActualPrice,
                    PromotionalText = p.PromotionalText,
                    ManningBookUrl = p.ManningBookUrl,

                    AuthorsOrdered = string.Join(", ",
                        p.AuthorsLink
                            .OrderBy(q => q.Order)
                            .Select(q => q.Author.Name)),
                    ReviewsCount = p.Reviews.Count(),
                    ReviewsAverageVotes =
                        p.Reviews.Select(y =>
                            (double?)y.NumStars).Average(),
                    Tags = p.Tags
                        .Select(x => new CosmosTag(x.TagId)).ToList(),
                    TagsString = $"| {string.Join(" | ", p.Tags.Select(x => x.TagId))} |"
                });
        }
    }
}