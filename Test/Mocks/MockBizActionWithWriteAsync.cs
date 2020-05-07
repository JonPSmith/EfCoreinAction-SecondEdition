// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizLogic.GenericInterfaces;
using DataLayer.EfClasses;
using DataLayer.EfCode;

namespace Test.Mocks
{
    public interface IMockBizActionWithWriteAsync : IBizActionAsync<MockBizActionWithWriteModes, string> {}


    public class MockBizActionWithWriteAsync : BizActionErrors, IMockBizActionWithWriteAsync
    {
        private readonly EfCoreContext _context;
        private readonly Guid _userId;

        public MockBizActionWithWriteAsync(EfCoreContext context, Guid userId)
        {
            _context = context;
            _userId = userId;
        }

        public Task<string> ActionAsync(MockBizActionWithWriteModes mode)
        {
            if (mode == MockBizActionWithWriteModes.BizError)
                AddError("There is a biz error.");

            var numBooks = (short) (mode == MockBizActionWithWriteModes.SaveChangesError ? 200 : 1);

            var order = new Order
            {
                CustomerId = _userId,
                LineItems = new List<LineItem>
                {
                    new LineItem
                    {
                        BookId = _context.Books.First().BookId,
                        LineNum = 1,
                        BookPrice = 123,
                        NumBooks = numBooks
                    }
                }
            };

            _context.Orders.Add(order);

            return Task.FromResult( mode.ToString());

        }
    }
}