// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;

namespace DataLayer.QueryObjects
{
    public static class GenericPaging
    {
        public static IQueryable<T> Page<T>(
            this IQueryable<T> query, 
            int pageNumZeroStart, int pageSize)
        {
            if (pageSize == 0)
                throw new ArgumentOutOfRangeException
                    (nameof(pageSize), "pageSize cannot be zero.");

            if (pageNumZeroStart != 0)
                query = query
                    .Skip(pageNumZeroStart * pageSize);    //#A

            return query.Take(pageSize);                   //#B
        }
        /***************************************************************
        #A It skips the correct number of pages
        #B It then takes the number for this page size
         * ************************************************************/
    }
}