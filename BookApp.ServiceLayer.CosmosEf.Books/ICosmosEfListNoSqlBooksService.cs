// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.ServiceLayer.DisplayCommon.Books;

namespace BookApp.ServiceLayer.CosmosEf.Books
{
    public interface ICosmosEfListNoSqlBooksService
    {
        Task<IList<CosmosBook>> SortFilterPageAsync(SortFilterPageOptionsNoCount options);
    }
}