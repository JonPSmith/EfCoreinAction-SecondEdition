// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using BookApp.Domain.Books;
using BookApp.ServiceLayer.DisplayCommon.Books;

namespace BookApp.ServiceLayer.CosmosEf.Books
{
    public class CosmosEfBookListCombinedDto
    {
        public CosmosEfBookListCombinedDto(SortFilterPageOptionsNoCount sortFilterPageData, IList<CosmosBook> booksList)
        {
            SortFilterPageData = sortFilterPageData;
            BooksList = booksList;
        }

        public SortFilterPageOptionsNoCount SortFilterPageData { get; private set; }

        public IList<CosmosBook> BooksList { get; private set; }
    }
}