// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookApp.Domain.Books;
using BookApp.Infrastructure.Book.Seeding;
using Newtonsoft.Json;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestInfrastructureBookSeeding
{
    public class TestManningBooksJson
    {
        private readonly ITestOutputHelper _output;

        public TestManningBooksJson(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ReadManningBooksJsonTagStatsOk()
        {
            //SETUP
            var callingAssemblyPath = TestData.GetCallingAssemblyTopLevelDir();
            var fileDir = Path.GetFullPath(Path.Combine(callingAssemblyPath, "..\\BookApp\\wwwroot\\seedData"));
            var filePath = Directory.GetFiles(fileDir, "ManningBooks*.json").FirstOrDefault();

            //ATTEMPT
            var list = JsonConvert.DeserializeObject<List<ManningBooksJson>>(File.ReadAllText(filePath));

            //VERIFY
            var allTags = list.SelectMany(x => x.tags ?? new string[0]).ToList();
            var tagDict = new Dictionary<string,int>();
            foreach (var tagInst in allTags)
            {
                if (!tagDict.ContainsKey(tagInst)) tagDict[tagInst] = 0;
                tagDict[tagInst] = tagDict[tagInst] + 1;
            }
            foreach (var key in tagDict.Keys)
            {
                _output.WriteLine($"{key.Length:d3}, {tagDict[key]:d3}: {key}");
            }
        }
    }
}