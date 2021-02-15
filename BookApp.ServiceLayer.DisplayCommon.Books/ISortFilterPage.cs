// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace BookApp.ServiceLayer.DisplayCommon.Books
{
    public interface ISortFilterPage
    {
        public OrderByOptions OrderByOptions { get; }

        public BooksFilterBy FilterBy { get; }

        public string FilterValue { get; }

        public int PageNum { get; }

        public int PageSize { get; }
        
        public bool NoCount { get; }
    }
}