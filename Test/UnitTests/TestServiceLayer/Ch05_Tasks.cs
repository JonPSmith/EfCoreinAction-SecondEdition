// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfCode;
using Test.TestHelpers;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestServiceLayer
{
    public class Ch05_Tasks
    {
        //Must be run on it own to check things
        [RunnableInDebugOnly]
        public async Task TwoTasksSameDbContextBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var task1 = MyTask(context);
                var task2 = MyTask(context);

                var exceptionRaised = false;
                try
                {
                    await Task.WhenAll(task1, task2);
                }
                catch (Exception e)
                {
                    e.ShouldBeType<InvalidOperationException>();
                    e.Message.ShouldEqual("A second operation started on this context before a previous operation completed. Any instance members are not guaranteed to be thread safe.");
                    exceptionRaised = true;
                }

                //VERIFY
                exceptionRaised.ShouldBeTrue();
            }
        }

        //Must be run on it own to check things
        [RunnableInDebugOnly]
        public async Task TwoTasksDifferentDbContextOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context1 = new EfCoreContext(options))
            using (var context2 = new EfCoreContext(options))
            {

                //ATTEMPT
                var task1 = MyTask(context1);
                var task2 = MyTask(context2);

                await Task.WhenAll(task1, task2);
            }
        }

        private async Task<int> MyTask(EfCoreContext context)
        {
            await Task.Delay(10);
            return context.Books.Count();
        }
    }
}