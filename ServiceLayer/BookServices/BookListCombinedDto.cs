// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using ServiceLayer.Logger;

namespace ServiceLayer.BookServices
{
    public class BookListCombinedDto : TraceIdentBaseDto
    {
        public BookListCombinedDto(string traceIdentifier,
            SortFilterPageOptions sortFilterPageData, IEnumerable<BookListDto> booksList)
            : base(traceIdentifier)
        {
            SortFilterPageData = sortFilterPageData;
            BooksList = booksList;
        }

        public SortFilterPageOptions SortFilterPageData { get; private set; }

        public IEnumerable<BookListDto> BooksList { get; private set; }
    }
}