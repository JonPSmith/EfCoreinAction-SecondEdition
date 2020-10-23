// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using BookApp.ServiceLayer.DefaultSql.Books;

namespace BookApp.ServiceLayer.DapperSql.Books.Dtos
{
    public class DapperBookListCombinedDto
    {
        public DapperBookListCombinedDto(SortFilterPageOptions sortFilterPageData, IEnumerable<DapperBookListDto> booksList)
        {
            SortFilterPageData = sortFilterPageData;
            BooksList = booksList;
        }

        public SortFilterPageOptions SortFilterPageData { get; private set; }

        public IEnumerable<DapperBookListDto> BooksList { get; private set; }
    }
}