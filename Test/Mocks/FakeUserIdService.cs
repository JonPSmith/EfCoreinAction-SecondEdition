// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using DataLayer.EfCode;

namespace Test.Mocks
{
    public class FakeUserIdService : IUserIdService
    {
        private readonly Guid _dataKey;

        public FakeUserIdService(Guid dataKey)
        {
            _dataKey = dataKey;
        }

        public Guid GetUserId()
        {
            return _dataKey;
        }
    }
}