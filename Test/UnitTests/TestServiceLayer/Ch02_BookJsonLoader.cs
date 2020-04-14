// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using ServiceLayer.DatabaseServices.Concrete;
using TestSupport.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestServiceLayer
{
    public class Ch02_BookJsonLoader
    {
        [Fact]
        public void TestBookLoadBuildReviewsOk()
        {
            //SETUP
            const string searchFile = "JsonBooks01*.json";
            var testDataDir = TestData.GetTestDataDir();

            //ATTEMPT
            var books = BookJsonLoader.LoadBooks(testDataDir, searchFile);

            //VERIFY
            var expectedAveVotes = new[] {5.0, 3.0, 4.0, 4.5};
            books.Select(x => x.Reviews.Average(y => y.NumStars)).ShouldEqual(expectedAveVotes);
        }

        [Fact]
        public void TestBookLoadOk()
        {
            //SETUP
            const string searchFile = "JsonBooks01*.json";
            var testDataDir = TestData.GetTestDataDir();

            //ATTEMPT
            var books = BookJsonLoader.LoadBooks(testDataDir, searchFile);

            //VERIFY
            books.Count().ShouldEqual(4);
        }
    }
}