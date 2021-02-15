// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using BookApp.ServiceLayer.DisplayCommon.Books;
using BookApp.ServiceLayer.UdfsSql.Books.Dtos;

namespace BookApp.ServiceLayer.UdfsSql.Books
{
    public interface IListUdfsBooksService
    {
        Task<IQueryable<UdfsBookListDto>> SortFilterPageAsync
            (SortFilterPageOptions options);
    }
}