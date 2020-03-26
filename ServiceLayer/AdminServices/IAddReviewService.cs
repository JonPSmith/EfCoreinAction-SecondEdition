// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfClasses;

namespace ServiceLayer.AdminServices
{
    public interface IAddReviewService
    {
        string BookTitle { get; }

        Review GetBlankReview(int id) //#A
            ;

        Book AddReviewToBook(Review review)//#D
            ;
    }
}