// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using BookApp.ServiceLayer.DisplayCommon.Books;
using BookApp.ServiceLayer.DisplayCommon.Books.Dtos;

namespace BookApp.ServiceLayer.CosmosEf.Books
{
    public interface ICosmosEfBookFilterDropdownService
    {
        /// <summary>
        /// This makes the various Value + text to go in the dropdown based on the FilterBy option
        /// </summary>
        /// <param name="filterBy"></param>
        /// <returns></returns>
        Task<IEnumerable<DropdownTuple>> GetFilterDropDownValuesAsync(BooksFilterBy filterBy);
    }
}