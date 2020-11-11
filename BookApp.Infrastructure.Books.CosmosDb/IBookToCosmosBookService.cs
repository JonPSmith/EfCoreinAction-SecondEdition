// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using BookApp.Domain.Books;

namespace BookApp.Infrastructure.Books.CosmosDb
{
    public interface IBookToCosmosBookService
    {
        Task AddCosmosBookAsync(Book sqlBook);
        Task UpdateCosmosBookAsync(Book sqlBook);
        Task DeleteCosmosBookAsync(Book sqlBook);
        Task UpdateManyCosmosBookAsync(List<int> bookIds);
    }
}