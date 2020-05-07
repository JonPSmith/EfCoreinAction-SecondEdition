// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BizDbAccess.Orders;
using BizLogic.Orders;
using BizLogic.Orders.Concrete;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using ServiceLayer.BizRunners;
using Test.Mocks;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestServiceLayer
{
    public class Ch04_RunnerTransact2WriteDb
    {
        [Theory]
        [InlineData(MockBizActionTransact2Modes.Ok)]
        [InlineData(MockBizActionTransact2Modes.BizErrorPart1)]
        [InlineData(MockBizActionTransact2Modes.BizErrorPart2)]
        public void RunAction(MockBizActionTransact2Modes mode)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var action1 = new MockBizActionPart1(context);
                var action2 = new MockBizActionPart2(context);
                var runner = new RunnerTransact2WriteDb<TransactBizActionDto, TransactBizActionDto, TransactBizActionDto>(context, action1, action2);

                //ATTEMPT
                var output = runner.RunAction(new TransactBizActionDto(mode));

                //VERIFY
                runner.HasErrors.ShouldEqual(mode != MockBizActionTransact2Modes.Ok);
                context.Authors.Count().ShouldEqual(mode != MockBizActionTransact2Modes.Ok ? 0 : 2);
                if (mode == MockBizActionTransact2Modes.BizErrorPart1)
                    runner.Errors.Single().ErrorMessage.ShouldEqual("Failed in Part1");
                if (mode == MockBizActionTransact2Modes.BizErrorPart2)
                    runner.Errors.Single().ErrorMessage.ShouldEqual("Failed in Part2");
            }
        }


        [Fact]
        public void ExampleCodeForBook()
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options, new FakeUserIdService(userId)))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks();
                var lineItems = new List<OrderLineItem>
                {
                    new OrderLineItem {BookId = 1, NumBooks = 4},
                    new OrderLineItem {BookId = 2, NumBooks = 5},
                    new OrderLineItem {BookId = 3, NumBooks = 6}
                };
                //ATTEMPT

                var dbAccess = new PlaceOrderDbAccess(context);
                var action1 = new PlaceOrderPart1(dbAccess);
                var action2 = new PlaceOrderPart2(dbAccess);
                var runner = new RunnerTransact2WriteDb<
                    PlaceOrderInDto, 
                    Part1ToPart2Dto, 
                    Order>(context, action1, action2);
                var order = runner.RunAction(new PlaceOrderInDto(true, userId, lineItems.ToImmutableList()));

                //VERIFY
                runner.HasErrors.ShouldBeFalse();
                context.Orders.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void RunActionThrowException()
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options, new FakeUserIdService(userId)))
            {
                context.Database.EnsureCreated();
                var action1 = new MockBizActionPart1(context);
                var action2 = new MockBizActionPart2(context);
                var runner = new RunnerTransact2WriteDb<TransactBizActionDto, TransactBizActionDto, TransactBizActionDto>(context, action1, action2);

                //ATTEMPT
                Assert.Throws<InvalidOperationException>(() => runner.RunAction(new TransactBizActionDto(MockBizActionTransact2Modes.ThrowExceptionPart2)));

                //VERIFY
                context.Authors.Count().ShouldEqual(0);
            }
        }
    }
}