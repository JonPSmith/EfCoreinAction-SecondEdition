// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Test.Chapter11Listings.EfClasses;
using Test.Chapter11Listings.EfCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch11_SaveChangesInterceptor
    {
        private readonly ITestOutputHelper _output;

        public Ch11_SaveChangesInterceptor(ITestOutputHelper output)
        {
            _output = output;
        }

        public class MySaveChangesInterceptor : SaveChangesInterceptor
        {
            private readonly bool _stopSaveChanges;

            public MySaveChangesInterceptor(bool stopSaveChanges)
            {
                _stopSaveChanges = stopSaveChanges;
            }

            public override InterceptionResult<int> SavingChanges(
                DbContextEventData eventData,
                InterceptionResult<int> result)
            {
                eventData.Context.ChangeTracker.Entries().Single().State.ShouldEqual(EntityState.Added);

                return _stopSaveChanges ? InterceptionResult<int>.SuppressWithResult(-1) : result;
            }

            public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
                DbContextEventData eventData,
                InterceptionResult<int> result,
                CancellationToken cancellationToken = new CancellationToken())
            {
                Console.WriteLine($"Saving changes asynchronously for {eventData.Context.Database.GetConnectionString()}");

                return _stopSaveChanges 
                    ? new ValueTask<InterceptionResult<int>>(InterceptionResult<int>.SuppressWithResult(-1))
                    : new ValueTask<InterceptionResult<int>>(result);
            }
        }

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, -1)]
        public void TestSaveChangesInterceptorOk(bool stopSaveChanges, int expectedResult)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>( builder => 
                builder.AddInterceptors(new MySaveChangesInterceptor(stopSaveChanges)));
            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new MyEntity {MyString = "Test"};
                context.Add(entity);   
                var result = context.SaveChanges();

                //VERIFY
                result.ShouldEqual(expectedResult);
                context.MyEntities.Count().ShouldEqual(stopSaveChanges ? 0 : 1);
            }
        }
        
    }
}
