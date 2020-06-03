// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter08Listings.EFCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch08_RelationshipBackingFields
    {
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
                    Title = "Quantem Networking"
                };

                //ATTEMPT
                var review = new Review {NumStars = 5, VoterName = "Unit Test"};
                entity.AddReview(review);
                entity.RemoveReview(review);

                //VERIFY
                entity.Reviews.Count().ShouldEqual(0);
                entity.CachedVotes.ShouldEqual(null);
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
                    Title = "Quantem Networking"
                };

                //VERIFY
                entity.Reviews.ShouldNotBeNull();
                entity.Reviews.Any().ShouldBeFalse();
                entity.CachedVotes.ShouldBeNull();
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
                entity.CachedVotes.ShouldBeNull();
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
                entity.CachedVotes.ShouldEqual(5);
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
                entity.CachedVotes.ShouldEqual(3);
            }
        }

        [Fact]
        public void TestSaveBookOneReviewAndReadOk()
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
                entity.AddReview(new Review {NumStars = 5, VoterName = "Unit Test"});
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter08DbContext(options))
            {
                //VERIFY
                var entity = context.Books.Include(x => x.Reviews).Single();
                entity.Reviews.ShouldNotBeNull();
                entity.Reviews.Count().ShouldEqual(1);
                entity.CachedVotes.ShouldEqual(5);
            }
        }
    }
}