// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.EfCode;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Test.Chapter02Listings;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch02_LazyLoading
    {
        private readonly ITestOutputHelper _output;

        public Ch02_LazyLoading(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestLazyLoadBookAndReviewUsingILazyLoaderOk()
        {
            //SETUP
            var showlog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<Lazy1DbContext>(log =>
            {
                if (showlog)
                    _output.WriteLine(log.Message);
            });
            using (var context = new Lazy1DbContext(options))
            {
                context.Database.EnsureCreated();
                var book = new BookLazy1
                {
                    Reviews = new List<LazyReview>
                    {
                        new LazyReview {NumStars = 5}, new LazyReview {NumStars = 1}
                    }
                };
                context.Add(book);
                context.SaveChanges();
            }
            using (var context = new Lazy1DbContext(options))
            {
                //ATTEMPT
                showlog = true;
                var book = context.BookLazy1s.Single(); //#A
                var reviews = book.Reviews.ToList(); //#B
                /*********************************************************
                #A This gets an instance of the BookLazy entity class that has configured its Reviews property to use lazy loading
                #B When the Reviews property is accessed, then EF Core will read in the reviews from the database
                * *******************************************************/

                //VERIFY
                reviews.Count.ShouldEqual(2);
            }
        }

        [Fact]
        public void TestLazyLoadBookAndReviewUsingProxiesPackageOk()
        {
            //SETUP
            var options = CreateOptionsLazyLoadingProxies<Lazy2DbContext>();
            using (var context = new Lazy2DbContext(options))
            {
                context.Database.EnsureCreated();
                var book = new BookLazy2
                {
                    Reviews = new List<LazyReview>
                    {
                        new LazyReview {NumStars = 5}, new LazyReview {NumStars = 1}
                    }
                };
                context.Add(book);
                context.SaveChanges();
            }
            using (var context = new Lazy2DbContext(options))
            {
                //ATTEMPT
                var book = context.BookLazy2s.Single(); //#A
                book.Reviews.Count().ShouldEqual(2); //#B
                /*********************************************************
                #A We just load the book class
                #B When the Reviews are read, then EF Core will read in the reviews
                * *******************************************************/
            }
        }

        public static DbContextOptions<T> CreateOptionsLazyLoadingProxies<T>() where T : DbContext
        {
            //Thanks to https://www.scottbrady91.com/Entity-Framework/Entity-Framework-Core-In-Memory-Testing
            var connectionStringBuilder =
                new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            connection.Open();                //see https://github.com/aspnet/EntityFramework/issues/6968

            // create in-memory context
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseLazyLoadingProxies()
                .UseSqlite(connection);

            return builder.Options;
        }
    }
}