// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using BookApp.Domain.Books;

namespace BookApp.ServiceLayer.CosmosEf.Books
{
    public class CosmosEfBookListCombinedDto
    {
        public CosmosEfBookListCombinedDto(CosmosEfSortFilterPageOptions sortFilterPageData, IList<CosmosBook> booksList)
        {
            SortFilterPageData = sortFilterPageData;
            BooksList = booksList;
        }

        public CosmosEfSortFilterPageOptions SortFilterPageData { get; private set; }

        public IList<CosmosBook> BooksList { get; private set; }
    }
}