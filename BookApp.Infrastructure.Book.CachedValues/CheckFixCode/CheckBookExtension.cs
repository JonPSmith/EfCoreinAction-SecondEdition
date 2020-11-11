// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;

namespace BookApp.Infrastructure.Books.CachedValues.CheckFixCode
{
    public static class CheckBookExtension
    {
        public static async Task<IStatusGeneric> CheckSingleBookAsync(
            this BookDbContext context, int bookId, bool fixBadCacheValues, CancellationToken cancellationToken)
        {
            var status = new StatusGenericHandler();

            var dto = await context.Books
                .IgnoreQueryFilters()
                .MapBookToDto()
                .SingleOrDefaultAsync(x => x.BookId == bookId, cancellationToken);
            if (dto == null)
                status.AddError("SQL: No book found.");

            Book loadedBook = null;
            var fixedThem = fixBadCacheValues ? "and fixed it" : "(not fixed)";

            if (dto.RecalcReviewsCount != dto.ReviewsCount || 
                Math.Abs((dto.RecalcReviewsAverageVotes ?? 0) - dto.ReviewsAverageVotes) > 0.0001)
            {
                status.AddError($"SQL: Review cached values incorrect {fixedThem}. " +
                                $"Actual Reviews.Count = {dto.RecalcReviewsCount}, Cached ReviewsCount = {dto.ReviewsCount}. " +
                                $"Actual Reviews average = {dto.RecalcReviewsAverageVotes:F5}, Cached ReviewsAverageVotes = {dto.ReviewsAverageVotes:F5}. " +
                                $"Last updated {dto.LastUpdatedUtc:G}");
                if (fixBadCacheValues)
                {
                    loadedBook = await context.Books.
                        SingleOrDefaultAsync(x => x.BookId == bookId);
                    loadedBook?.UpdateReviewCachedValues(dto.RecalcReviewsCount, dto.RecalcReviewsAverageVotes ?? 0);
                }
            }

            if (dto.RecalcAuthorsOrdered != dto.AuthorsOrdered)
            {
                status.AddError($"SQL: AuthorsOrdered cached value incorrect {fixedThem}. " +
                                $"Actual authors string = {dto.RecalcAuthorsOrdered}, Cached AuthorsOrdered = {dto.AuthorsOrdered}. " +
                                $"Last updated {dto.LastUpdatedUtc:G}");

                if (fixBadCacheValues)
                {
                    loadedBook ??= await context.Books.SingleOrDefaultAsync(x => x.BookId == bookId);
                    loadedBook.ResetAuthorsOrdered(dto.RecalcAuthorsOrdered);
                }
            }

            return status;
        }
    }
}