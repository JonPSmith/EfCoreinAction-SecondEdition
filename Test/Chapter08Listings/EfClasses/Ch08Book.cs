// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using DataLayer.EfClasses;

namespace Test.Chapter08Listings.EfClasses
{
    public class Ch08Book
    {
        private readonly List<Review> _reviews = new List<Review>(); //#A

        public int BookId { get; set; }
        public string Title { get; set; }

        public double? ReviewsAverageVotes { get; private set; } //#B

        public IReadOnlyCollection<Review> Reviews => _reviews.ToImmutableList(); //#C

        public void AddReview(Review review) //#D
        {
            _reviews.Add(review); //#E
            ReviewsAverageVotes = 
                _reviews.Average(x => x.NumStars); //#F
        }

        public void RemoveReview(Review review) //#G
        {
            _reviews.Remove(review); //#H
            ReviewsAverageVotes = _reviews.Any()
                ? _reviews.Average(x => x.NumStars) //#I
                : (double?)null; //#J
        }
    }
    /*********************************************************
    #A You add a backing field, which is a list. By default EF Core will read and write to this
    #B This holds a pre-calculated average of the reviews. Note that it is read-only so that it cannot be changed outside this class
    #C This returns an immutable copy of the reviews that were loaded. 
    #D You add a method to allow a new Review to be added to the _reviews collection
    #E You add the new review to the backing field _reviews. This will update the database on the call to SaveChanges
    #F Then you recalculate the average votes for the book
    #G You add a method to remove a review from the _reviews collection
    #H This removes the review from the list. This will update the database on the call to SaveChanges
    #I If there are any reviews you recalculate the average votes for the book
    #J If there are no reviews you set the value to null
    * *********************************************************/
}