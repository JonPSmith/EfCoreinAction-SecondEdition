// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace BookApp.Infrastructure.Books.CachedValues
{
    public interface ICheckFixCacheValuesService
    {
        Task RunCheckAsync(DateTime fromThisDate, bool fixBadCacheValues, CancellationToken cancellationToken);
    }
}