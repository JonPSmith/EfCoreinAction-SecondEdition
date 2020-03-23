// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using Xunit;
using EfCoreInAction.DatabaseHelpers;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.AspNetCore
{
    public class TestCalculateReviewsToMatch
    {
        [Theory]
        [InlineData(5)]
        [InlineData(5,1)]
        [InlineData(5, 1, 1)]
        [InlineData(5, 1, 1)]
        [InlineData(5, 1, 1, 1, 1)]
        [InlineData(5, 1, 5)]
        [InlineData(3, 4, 3)]
        public void TestCalcReviewsOk(params int [] voteVals)
        {
            //SETUP
            var avgRating = voteVals.Average();
            var numVotes = voteVals.Length;

            //ATTEMPT
            var reviews = BookJsonLoader.CalculateReviewsToMatch(avgRating, numVotes);

            //VERIFY
            reviews.Count.ShouldEqual(numVotes);
            reviews.Average(x => x.NumStars).ShouldEqual(avgRating);
        }
    }
}