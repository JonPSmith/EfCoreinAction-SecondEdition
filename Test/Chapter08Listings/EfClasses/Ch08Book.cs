// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DataLayer.EfClasses;

namespace Test.Chapter08Listings.EfClasses
{
    public class Ch08Book
    {
        private readonly IList<Review> _reviews = new List<Review>(); //#A

        [Key]
        public int BookId { get; set; }
        public string Title { get; set; }

        public double? ReviewsAverageVotes { get; private set; }      //#B

        public IReadOnlyCollection<Review> Reviews =>                 //#C
            _reviews.ToImmutableList();                               //#D

        public void AddReview(Review review)                          //#E
        {
            _reviews.Add(review);                                     //#F
            ReviewsAverageVotes = 
                _reviews.Average(x => x.NumStars);                    //#G
        }

        public void RemoveReview(Review review)                       //#H
        {
            _reviews.Remove(review);                                  //#I
            ReviewsAverageVotes = _reviews.Any()
                ? _reviews.Average(x => x.NumStars)                   //#J
                : (double?)null;                                      //#K
        }
    }
    /*********************************************************
    #A You add a backing field, which is a list. By default EF Core will read and write to this
    #B This holds a pre-calculated average of the reviews. This is read-only
    #C This is a read-only collection so no one can change the collection
    #D This returns an immutable copy of the reviews in the _reviews backing field. 
    #E You add a method to allow a new Review to be added to the _reviews collection
    #F You add the new review to the backing field _reviews. This will update the database on the call to SaveChanges
    #G Then you recalculate the average votes for the book
    #H You add a method to remove a review from the _reviews collection
    #I This removes the review from the list. This will update the database on the call to SaveChanges
    #J If there are any reviews you recalculate the average votes for the book
    #K If there are no reviews you set the value to null
    * *********************************************************/
}