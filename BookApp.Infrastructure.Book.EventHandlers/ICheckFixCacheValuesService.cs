// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace BookApp.Infrastructure.Books.EventHandlers
{
    public interface ICheckFixCacheValuesService
    {
        Task<DateTime> RunCheckAsync(DateTime fromThisDate);
    }
}