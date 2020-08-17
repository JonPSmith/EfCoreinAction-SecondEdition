// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using BookApp.Domain.Books;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("Test")]

namespace BookApp.Infrastructure.Books.Seeding
{
    /// <summary>
    /// This reads in the Manning book info from json and converts it to a series of Books, Authors, and Tags
    /// The stages are:
    /// 1. Read in all the Manning books and extract all the Authors and Tags
    /// </summary>
    public static class ManningJsonToBooks
    {
        private const string ImageUrlPrefix = "https://images.manning.com/360/480/resize/";

        public static IEnumerable<Book> LoadBooks(this string fileDir, string fileSearchString)
        {
            var filePath = GetJsonFilePath(fileDir, fileSearchString);
            var jsonDecoded = JsonConvert.DeserializeObject<List<ManningBooksJson>>(File.ReadAllText(filePath));

            var tagsNames = jsonDecoded.SelectMany(x => x.tags ?? new string[0]).Distinct();
            var tagDict = tagsNames.ToDictionary(x => x, y => new Tag(y));
            var authorDict = new Dictionary<string,Author>();
            foreach (var manningBooksJson in jsonDecoded)
            {
                var authors = manningBooksJson.NormalizeAuthorNames().ToList();
                manningBooksJson.authorshipDisplay = authors.Any()
                    ? string.Join(',', authors)
                    : null;
                authors.ForEach(name =>
                {
                    if (!authorDict.ContainsKey(name))
                        authorDict[name] = new Author(name, null);
                });
            }

            foreach (var jsonBook in jsonDecoded.Where(x => x.authorshipDisplay != null))
            {
                var fullImageUrl = ImageUrlPrefix + jsonBook.imageUrl;
                var publishedOn = jsonBook.publishedDate ?? jsonBook.expectedPublishDate;
                var price = jsonBook.productOfferings.Any()
                    ? jsonBook.productOfferings.Select(x => x.price).Max()
                    : 100;
                var authors = jsonBook.authorshipDisplay.Split(',')
                    .Select(x => authorDict[x]).ToList();
                var tags = (jsonBook.tags ?? new string[0])
                    .Select(x => tagDict[x]).ToList();

                var status = Book.CreateBook(jsonBook.title, null, publishedOn,
                    "Manning", (decimal) price, fullImageUrl, authors, tags);
                if (status.HasErrors)
                    throw new InvalidOperationException( $"Book {jsonBook.title}: errors = {status.GetAllErrors()}");

                yield return status.Result;
            }
        }

        internal static IEnumerable<string> NormalizeAuthorNames(this ManningBooksJson json)
        {
            const string withChaptersBy = "With chapters selected by";
            //There are ??? formats
            //- Author1
            //- Author1 and Author2
            //- Author1, Author2
            //- Author1, Author2 with Author3
            //- Author1<br><i>Foreword by ...
            //- With chapters selected by ...
            //- contributions by
            //- Author1, Ph.D.
            //- null 

            if (json.authorshipDisplay == null)
                return new string[0];

            var authorString = json.authorshipDisplay.StartsWith(withChaptersBy)
                ? json.authorshipDisplay.Substring(withChaptersBy.Length)
                : json.authorshipDisplay;

            var breakIndex = authorString.IndexOf("<");
            if (breakIndex > 0)
                authorString = authorString.Substring(0, breakIndex);
            var editedIndex = authorString.IndexOf("Edited by");
            if (editedIndex > 0)
                authorString = authorString.Substring(0, editedIndex);

            authorString = authorString
                .Replace("  ", " ")
                .Replace("Ph.D.","")
                .Replace("contributions by", ",")
                .Replace(" with ", ",")
                .Replace(" and ", ",");
            if(Regex.Match(authorString, @";|#|&").Success)
                return new string[0];

            var authors = authorString.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).Select(y => y.Trim());

            return authors;
        }

        private static string GetJsonFilePath(string fileDir, string searchPattern)
        {
            var fileList = Directory.GetFiles(fileDir, searchPattern);

            if (fileList.Length == 0)
                throw new FileNotFoundException(
                    $"Could not find a file with the search name of {searchPattern} in directory {fileDir}");

            //If there are many then we take the most recent
            return fileList.ToList().OrderBy(x => x).Last();
        }
    }
}