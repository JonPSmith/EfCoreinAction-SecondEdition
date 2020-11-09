// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace BookApp.Infrastructure.Books.CachedValues.CheckFixCode
{
    public class CheckFixBookDto
    {
        public int BookId { get; set; }

        //Cached values
        public string AuthorsOrdered { get; set; }
        public int ReviewsCount { get;  set; }
        public double ReviewsAverageVotes { get;  set; }

        //Recalculated values
        public string RecalcAuthorsOrdered { get; set; }
        public int RecalcReviewsCount { get;  set; }
        public double? RecalcReviewsAverageVotes { get;  set; }

        public DateTime LastUpdatedUtc { get; set; }
    }
}