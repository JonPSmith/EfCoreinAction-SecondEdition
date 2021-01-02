// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter08Listings.EFCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;
using Xunit.Sdk;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch08_RelationshipBackingFields
    {
        private readonly ITestOutputHelper _output;

        public Ch08_RelationshipBackingFields(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestCreateBookAddRemoveOneReviewOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new Ch08Book
                {
                    Title = "Quantum Networking"

                };

                //ATTEMPT
                var review = new Review {NumStars = 5, VoterName = "Unit Test"};
                entity.AddReview(review);
                entity.RemoveReview(review);

                //VERIFY
                entity.Reviews.Count().ShouldEqual(0);
                entity.ReviewsAverageVotes.ShouldEqual(null);
            }
        }

        [Fact]
        public void TestCreateBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new Ch08Book
                {
                    Title = "Quantum Networking"
                };

                //VERIFY
                entity.Reviews.ShouldNotBeNull();
                entity.Reviews.Any().ShouldBeFalse();
                entity.ReviewsAverageVotes.ShouldBeNull();
            }
        }

        [Fact]
        public void TestCreateBookOneAddedOneRemovedReviewOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new Ch08Book
                {
                    Title = "Quantum Networking"
                };
                var review = new Review {NumStars = 5, VoterName = "Unit Test"};
                entity.AddReview(review);
                entity.RemoveReview(review);

                //VERIFY
                entity.Reviews.ShouldNotBeNull();
                entity.Reviews.Count().ShouldEqual(0);
                entity.ReviewsAverageVotes.ShouldBeNull();
            }
        }

        [Fact]
        public void TestCreateBookOneReviewOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new Ch08Book
                {
                    Title = "Quantum Networking"
                };
                entity.AddReview(new Review{NumStars = 5, VoterName = "Unit Test"});

                //VERIFY
                entity.Reviews.ShouldNotBeNull();
                entity.Reviews.Count().ShouldEqual(1);
                entity.ReviewsAverageVotes.ShouldEqual(5);
            }
        }

        [Fact]
        public void TestCreateBookTwoReviewOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new Ch08Book
                {
                    Title = "Quantem Networking"
                };
                entity.AddReview(new Review { NumStars = 4, VoterName = "Unit Test" });
                entity.AddReview(new Review { NumStars = 2, VoterName = "Unit Test" });

                //VERIFY
                entity.Reviews.ShouldNotBeNull();
                entity.Reviews.Count().ShouldEqual(2);
                entity.ReviewsAverageVotes.ShouldEqual(3);
            }
        }

        [Fact]
        public void TestSaveBookOneReviewAndReadOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using var context = new Chapter08DbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var entity = new Ch08Book
            {
                Title = "Quantum Networking"
            };
            entity.AddReview(new Review {NumStars = 5, VoterName = "Unit Test"});
            context.Add(entity);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //VERIFY
            var book = context.Books.Include(x => x.Reviews).Single();
            book.Reviews.ShouldNotBeNull();
            book.Reviews.Count().ShouldEqual(1);
            book.ReviewsAverageVotes.ShouldEqual(5);
        }

        [Fact]
        public void TestListPerformanceAsReadOnlyVsToImmutableList()
        {
            const int listSize = 1000;
            var bigList = new List<Review>();
            for (int i = 0; i < listSize; i++)
            {
                bigList.Add(new Review());
            }

            using (new TimeThings(_output, "AsReadOnly", listSize))
            {
                var readOnly = bigList.AsReadOnly();
            }
            using (new TimeThings(_output, "ToImmutableList", listSize))
            {
                var readOnly = bigList.ToImmutableList();
            }
            using (new TimeThings(_output, "AsReadOnly", listSize))
            {
                var readOnly = bigList.AsReadOnly();
            }
            using (new TimeThings(_output, "ToImmutableList", listSize))
            {
                var readOnly = bigList.ToImmutableList();
            }
        }

        [Fact]
        public void TestHasSetPerformanceAsReadOnlyVsToImmutableList()
        {
            const int listSize = 1000;
            var bigList = new HashSet<Review>();
            for (int i = 0; i < listSize; i++)
            {
                bigList.Add(new Review());
            }

            using (new TimeThings(_output, "AsReadOnly", listSize))
            {
                var readOnly = bigList.ToList().AsReadOnly();
            }
            using (new TimeThings(_output, "ToImmutableList", listSize))
            {
                var readOnly = bigList.ToImmutableList();
            }
            using (new TimeThings(_output, "AsReadOnly", listSize))
            {
                var readOnly = bigList.ToList().AsReadOnly();
            }
            using (new TimeThings(_output, "ToImmutableList", listSize))
            {
                var readOnly = bigList.ToImmutableList();
            }
        }
    }
}