// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using BookApp.Infrastructure.Books.Seeding;
using TestSupport.Helpers;
using Xunit;

namespace Test.UnitTests.TestInfrastructureBookSeeding
{
    public class TestManningJsonToBooks
    {
        [Fact]
        public void ReadManningBooksJsonListAuthorsDuplicateOk()
        {
            //SETUP
            var callingAssemblyPath = TestData.GetCallingAssemblyTopLevelDir();
            var fileDir = Path.GetFullPath(Path.Combine(callingAssemblyPath, "..\\BookApp\\wwwroot\\seedData"));

            //ATTEMPT
            var books = fileDir.LoadBooks("ManningBooks*.json").ToList();

            //VERIFY

        }
    }
}