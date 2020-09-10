// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter12Listings.BusinessLogic
{
    public class DummyWarehouseService : IWarehouseEventHandler
    {
        public bool NeedsCallToWarehouse(DbContext context)
        {
            return false;
        }

        public List<string> AllocateOrderAndDispatch()
        {
            return new List<string>();
        }
    }
}