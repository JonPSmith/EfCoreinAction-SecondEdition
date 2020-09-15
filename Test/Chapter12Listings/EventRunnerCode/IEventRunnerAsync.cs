// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter12Listings.EventRunnerCode
{
    public interface IEventRunnerAsync
    {
        Task RunEventsAsync(DbContext context);
    }
}