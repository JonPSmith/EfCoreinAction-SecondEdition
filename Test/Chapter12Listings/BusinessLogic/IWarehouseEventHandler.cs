// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter12Listings.BusinessLogic
{
    public interface IWarehouseEventHandler
    {
        bool NeedsCallToWarehouse(DbContext context);
        List<string> AllocateOrderAndDispatch();
    }
}