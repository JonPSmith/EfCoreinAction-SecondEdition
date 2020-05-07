// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfCode;
using ServiceLayer.BizRunners;
using Test.Mocks;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestServiceLayer
{
    public class Ch04_RunnerWriteDbWithValidation
    {
        [Theory]
        [InlineData(MockBizActionWithWriteModes.Ok)]
        [InlineData(MockBizActionWithWriteModes.BizError)]
        [InlineData(MockBizActionWithWriteModes.SaveChangesError)]
        public void RunAction(MockBizActionWithWriteModes mode)
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options, new FakeUserIdService(userId)))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var action = new MockBizActionWithWrite(context, userId);
                var runner = new RunnerWriteDbWithValidation<MockBizActionWithWriteModes, string>(action, context);

                //ATTEMPT
                var output = runner.RunAction(mode);

                //VERIFY
                output.ShouldEqual(mode.ToString());
                runner.HasErrors.ShouldEqual(mode != MockBizActionWithWriteModes.Ok);
                context.Orders.Count().ShouldEqual(mode != MockBizActionWithWriteModes.Ok ? 0 : 1);

                if (mode == MockBizActionWithWriteModes.BizError)
                    runner.Errors.Single().ErrorMessage.ShouldEqual("There is a biz error.");
                if (mode == MockBizActionWithWriteModes.SaveChangesError)
                    runner.Errors.Single().ErrorMessage.ShouldEqual("If you want to order a 100 or more books please phone us on 01234-5678-90");
            }
        }
    }
}