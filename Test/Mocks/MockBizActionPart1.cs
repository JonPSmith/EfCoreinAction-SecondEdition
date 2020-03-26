// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using BizLogic.GenericInterfaces;
using DataLayer.EfClasses;
using DataLayer.EfCode;

namespace Test.Mocks
{
    public interface IMockBizActionPart1 : IBizAction<TransactBizActionDto, TransactBizActionDto> {}

    public class MockBizActionPart1 : BizActionErrors, IMockBizActionPart1
    {
        private readonly EfCoreContext _context;

        public MockBizActionPart1(EfCoreContext context)
        {
            _context = context;
        }

        public TransactBizActionDto Action(TransactBizActionDto dto)
        {
            if (dto.Mode == MockBizActionTransact2Modes.BizErrorPart1)
                AddError("Failed in Part1");

            _context.Authors.Add(new Author {Name = "Part1"});

            return dto;

        }
    }
}