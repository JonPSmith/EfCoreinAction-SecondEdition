// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookApp.Domain.Books;
using BookApp.Infrastructure.Books.Seeding;
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
        public void ReadManningBooksJsonCheckPublishedDatesOk()
        {
            //SETUP
            var callingAssemblyPath = TestData.GetCallingAssemblyTopLevelDir();
            var fileDir = Path.GetFullPath(Path.Combine(callingAssemblyPath, "..\\BookApp.UI\\wwwroot\\seedData"));
            var filePath = Directory.GetFiles(fileDir, "ManningBooks*.json").FirstOrDefault();

            //ATTEMPT
            var list = JsonConvert.DeserializeObject<List<ManningBooksJson>>(File.ReadAllText(filePath));

            //VERIFY
            foreach (var json in list)
            {
                if (json.expectedPublishDate != json.publishedDate && json.publishedDate != null)
                {
                    _output.WriteLine($"expected = {json.expectedPublishDate:d}, pub = {json.publishedDate:d}. id = {json.id:D4}, name = {json.title}");
                }
            }
        }

        [Fact]
        public void ReadManningBooksJsonListAuthorsDisplayOk()
        {
            //SETUP
            var callingAssemblyPath = TestData.GetCallingAssemblyTopLevelDir();
            var fileDir = Path.GetFullPath(Path.Combine(callingAssemblyPath, "..\\BookApp.UI\\wwwroot\\seedData"));
            var filePath = Directory.GetFiles(fileDir, "ManningBooks*.json").FirstOrDefault();

            //ATTEMPT
            var list = JsonConvert.DeserializeObject<List<ManningBooksJson>>(File.ReadAllText(filePath));

            //VERIFY
            var allAuthors = list.Select(x => x.authorshipDisplay).ToList();
            foreach (var authors in allAuthors)
            {
                _output.WriteLine(authors ?? "<null>");
            }
        }

        [Fact]
        public void ReadManningBooksJsonListAuthorsDisplayConvertedOk()
        {
            //SETUP
            var callingAssemblyPath = TestData.GetCallingAssemblyTopLevelDir();
            var fileDir = Path.GetFullPath(Path.Combine(callingAssemblyPath, "..\\BookApp.UI\\wwwroot\\seedData"));
            var filePath = Directory.GetFiles(fileDir, "ManningBooks*.json").FirstOrDefault();

            //ATTEMPT
            var list = JsonConvert.DeserializeObject<List<ManningBooksJson>>(File.ReadAllText(filePath));

            //VERIFY
            foreach (var json in list)
            {
                _output.WriteLine(string.Join('|', json.NormalizeAuthorNames()) );
            }
        }

        [Fact]
        public void ReadManningBooksJsonListAuthorsDuplicateOk()
        {
            //SETUP
            var callingAssemblyPath = TestData.GetCallingAssemblyTopLevelDir();
            var fileDir = Path.GetFullPath(Path.Combine(callingAssemblyPath, "..\\BookApp.UI\\wwwroot\\seedData"));
            var filePath = Directory.GetFiles(fileDir, "ManningBooks*.json").FirstOrDefault();

            //ATTEMPT
            var list = JsonConvert.DeserializeObject<List<ManningBooksJson>>(File.ReadAllText(filePath));

            //VERIFY
            var authorsDict = new Dictionary<string, ManningBooksJson>();
            foreach (var json in list)
            {
                var authors = json.NormalizeAuthorNames().ToList();
                if (!authors.Any()) continue;
                foreach (var author in authors)
                {
                    if (authorsDict.ContainsKey(author))
                        _output.WriteLine($"Duplicate author {author}. This books = {json.title}, last book = {authorsDict[author].title}");
                    authorsDict[author] = json;
                }
            }
        }

        [Fact]
        public void ReadManningBooksJsonTagStatsOk()
        {
            //SETUP
            var callingAssemblyPath = TestData.GetCallingAssemblyTopLevelDir();
            var fileDir = Path.GetFullPath(Path.Combine(callingAssemblyPath, "..\\BookApp.UI\\wwwroot\\seedData"));
            var filePath = Directory.GetFiles(fileDir, "ManningBooks*.json").FirstOrDefault();

            //ATTEMPT
            var list = JsonConvert.DeserializeObject<List<ManningBooksJson>>(File.ReadAllText(filePath));

            //VERIFY
            var allTags = list.SelectMany(x => x.tags ?? new string[0]).ToList();
            var tagDict = new Dictionary<string, int>();
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