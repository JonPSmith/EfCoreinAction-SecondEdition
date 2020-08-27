// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Test.Chapter12Listings.EventInterfacesEtc
{
    public interface IEventRunner
    {
        void RunEvents(DbContext context);
    }
}