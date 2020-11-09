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
                status.AddError($"BookId: {dto.BookId}: No book found.");

            Book loadedBook = null;

            if (dto.RecalcReviewsCount != dto.ReviewsCount || 
                Math.Abs((dto.RecalcReviewsAverageVotes ?? 0) - dto.ReviewsAverageVotes) > 0.0001)
            {
                status.AddError($"BookId: {dto.BookId}, Review cached values incorrect\n" +
                                  $"Actual Reviews.Count = {dto.RecalcReviewsCount}, Cached ReviewsCount = {dto.ReviewsCount}\n" +
                                  $"Actual Reviews average = {dto.RecalcReviewsAverageVotes:F5}, Cached ReviewsAverageVotes = {dto.ReviewsAverageVotes:F5}\n" +
                                  $"Last updated {dto.LastUpdatedUtc:G}");
                if (fixBadCacheValues)
                {
                    loadedBook = await context.Books.
                        SingleOrDefaultAsync(x => x.BookId == bookId);
                    loadedBook?.UpdateReviewCachedValues(dto.RecalcReviewsCount, dto.RecalcReviewsAverageVotes ?? 0);
                    status.AddError($"BookId: {dto.BookId}: Review cached values fixed.");
                }
            }

            if (dto.RecalcAuthorsOrdered != dto.AuthorsOrdered)
            {
                status.AddError($"BookId: {dto.BookId}, AuthorsOrdered cached value incorrect\n" +
                                $"Actual authors string = {dto.RecalcAuthorsOrdered}, Cached AuthorsOrdered = {dto.AuthorsOrdered}\n" +
                                $"Last updated {dto.LastUpdatedUtc:G}");

                if (fixBadCacheValues)
                {
                    loadedBook ??= await context.Books.SingleOrDefaultAsync(x => x.BookId == bookId);
                    loadedBook.AuthorsOrdered = dto.RecalcAuthorsOrdered;
                    status.AddError($"BookId: {dto.BookId}: AuthorsOrdered cached value fixed.");
                }
            }

            return status;
        }
    }
}