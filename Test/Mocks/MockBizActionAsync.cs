// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading.Tasks;
using BizLogic.GenericInterfaces;
using DataLayer.EfClasses;
using DataLayer.EfCode;

namespace Test.Mocks
{
    public interface IMockBizActionAsync : IBizActionAsync<int, string> {}

    public class MockBizActionAsync : BizActionErrors, IMockBizActionAsync
    {
        private readonly EfCoreContext _context;

        public MockBizActionAsync(EfCoreContext context)
        {
            _context = context;
        }

        public Task<string> ActionAsync(int intIn)
        {
            if (intIn < 0)
                AddError("The intInt is less than zero");

            _context.Authors.Add(new Author {Name = "MockBizAction"});

            return Task.FromResult(intIn.ToString());

        }
    }
}