// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using DataLayer.EfCode;

namespace Test.Mocks
{
    public class FakeDataKeyService : IDataKeyService
    {
        private readonly Guid _dataKey;

        public FakeDataKeyService(Guid dataKey)
        {
            _dataKey = dataKey;
        }

        public Guid GetDataKey()
        {
            return _dataKey;
        }
    }
}