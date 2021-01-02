// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Test.Mocks;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch07_EfCoreContextConfig
    {
        private readonly ITestOutputHelper _output;

        public Ch07_EfCoreContextConfig(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestCheckBookConfigOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {

                //ATTEMPT

                //VERIFY
                context.GetColumnStoreType<Book>(nameof(Book.Title)).ShouldEqual("nvarchar(256)");
                context.GetColumnStoreType<Book>(nameof(Book.Publisher)).ShouldEqual("nvarchar(64)");
                context.GetColumnStoreType<Book>(nameof(Book.PublishedOn)).ShouldEqual("date");
                context.GetColumnStoreType<Book>(nameof(Book.Price)).ShouldEqual("decimal(9,2)");
                context.GetColumnStoreType<Book>(nameof(Book.ImageUrl)).ShouldEqual("varchar(512)");
            }
        }


        [Fact]
        public void TestOrderQueryFilterOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            options.StopNextDispose();
            var userId1 = Guid.NewGuid();
            using (var context = new EfCoreContext(options, new FakeUserIdService(userId1)))
            {
                context.Database.EnsureCreated();
                context.Add(new Order {UserId = userId1});
                context.SaveChanges();
                context.Orders.Count().ShouldEqual(1);
            }
            
            //ATTEMPT
            using (var context = new EfCoreContext(options, new FakeUserIdService(Guid.Empty)))
            {

                //VERIFY
                context.Orders.Count().ShouldEqual(0);
            }
        }

    }
}