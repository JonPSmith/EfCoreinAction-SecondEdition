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
    public class ManningBookLoad
    {
        private const string ImageUrlPrefix = "https://images.manning.com/360/480/resize/";
        private const string ManningUrlWithParam = "https://www.manning.com/books/{0}?a_aid=su4utaraxuTre8tuthup";

        public const string PublisherString = "Manning publications";
        const string OriginalBooksTag = "Manning books";

        private readonly string _fileDir;
        private readonly string _summarySearchString;
        private readonly string _detailSearchString;

        public ManningBookLoad(string fileDir, string summarySearchString, string detailSearchString)
        {
            _fileDir = fileDir ?? throw new ArgumentNullException(nameof(fileDir));
            _summarySearchString = summarySearchString ?? throw new ArgumentNullException(nameof(summarySearchString));
            _detailSearchString = detailSearchString ?? throw new ArgumentNullException(nameof(detailSearchString));
        }

        /// <summary>
        /// This returns Book entities created using the Manning json data
        /// </summary>
        /// <param name="tagAsOriginal">if true, then add an extra tag to say they are Manning books</param>
        /// <returns></returns>
        public IEnumerable<Book> LoadBooks(bool tagAsOriginal)
        {
            var summaryFilePath = GetJsonFilePath(_fileDir, _summarySearchString);
            var summaryJson = JsonConvert.DeserializeObject<List<ManningBooksJson>>(File.ReadAllText(summaryFilePath));
            var detailFilePath = GetJsonFilePath(_fileDir, _detailSearchString);
            var detailDict = JsonConvert.DeserializeObject<List<ManningDetailsJson>>(File.ReadAllText(detailFilePath))
                .ToDictionary(x => x.productId);

            var tagsNames = summaryJson.SelectMany(x => x.tags ?? new string[0]).Distinct().ToList();
            if (tagAsOriginal)
                tagsNames.Add(OriginalBooksTag);
            var tagsDict = tagsNames.ToDictionary(x => x, y => new Tag(y));
            var authorsDict = NormalizeAuthorsToCommaDelimited(summaryJson);

            foreach (var jsonBook in summaryJson.Where(x => x.authorshipDisplay != null))
            {
                var fullImageUrl = ImageUrlPrefix + jsonBook.imageUrl;
                var publishedOn = jsonBook.publishedDate ?? jsonBook.expectedPublishDate;
                var price = jsonBook.productOfferings.Any()
                    ? jsonBook.productOfferings.Select(x => x.price).Max()
                    : 100;
                var authors = jsonBook.authorshipDisplay.Split(',')
                    .Select(x => authorsDict[x]).ToList();
                var tags = (jsonBook.tags ?? new string[0])
                    .Select(x => tagsDict[x]).ToList();
                if (tagAsOriginal)
                    tags.Add(tagsDict[OriginalBooksTag]);

                var status = Book.CreateBook(jsonBook.title, publishedOn, jsonBook.publishedDate == null,
                    PublisherString, (decimal)price, fullImageUrl, authors, tags);
                if (status.HasErrors)
                    throw new InvalidOperationException($"Book {jsonBook.title}: errors = {status.GetAllErrors()}");

                status.Result.SetManningBookUrl(string.Format(ManningUrlWithParam, jsonBook.slug));

                if (detailDict.ContainsKey(jsonBook.id))
                {
                    var summary = detailDict[jsonBook.id];
                    status.Result.SetBookDetails(summary.description, summary.aboutAuthor,
                        summary.aboutReader, summary.aboutTechnology, summary.whatsInside);
                }

                yield return status.Result;
            }
        }

        private static Dictionary<string, Author> NormalizeAuthorsToCommaDelimited(List<ManningBooksJson> summaryJson)
        {
            var authorDict = new Dictionary<string, Author>();
            foreach (var manningBooksJson in summaryJson)
            {
                var authors = NormalizeAuthorNames(manningBooksJson).ToList();
                manningBooksJson.authorshipDisplay = authors.Any()
                    ? string.Join(',', authors)
                    : null;
                authors.ForEach(name =>
                {
                    if (!authorDict.ContainsKey(name))
                        authorDict[name] = new Author(name, null);
                });
            }

            return authorDict;
        }

        //This decodes The authorshipDisplay string which contains lots of different formats
        internal static IEnumerable<string> NormalizeAuthorNames(ManningBooksJson json)
        {
            const string withChaptersBy = "With chapters selected by";
            //The formats for authors are
            //- Author1
            //- Author1 and Author2
            //- Author1, Author2
            //- Author1, Author2 with Author3
            //- Author1<br><i>Foreword by ...
            //- Author1 Edited by
            //- With chapters selected by ...
            //- contributions by
            //- Author1, Ph.D.
            //- null 

            if (json.authorshipDisplay == null)
                return new string[0];

            var authorString = json.authorshipDisplay.StartsWith(withChaptersBy)
                ? json.authorshipDisplay.Substring(withChaptersBy.Length)
                : json.authorshipDisplay;

            var breakIndex = authorString.IndexOf("<"); //<br><i>Foreword by 
            if (breakIndex > 0)
                authorString = authorString.Substring(0, breakIndex);
            var editedIndex = authorString.IndexOf("Edited by");
            if (editedIndex > 0)
                authorString = authorString.Substring(0, editedIndex);

            authorString = authorString
                .Replace("  ", " ")
                .Replace("Ph.D.", "")
                .Replace("contributions by", ",")
                .Replace(" with ", ",")
                .Replace(" and ", ",");
            if (Regex.Match(authorString, @";|#|&").Success)//Some name come out wrong - don't know why
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