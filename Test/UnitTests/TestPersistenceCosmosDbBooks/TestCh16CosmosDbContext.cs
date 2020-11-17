// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Test.Chapter16Listings;
using Test.TestHelpers;
using TestSupport.Attributes;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestPersistenceCosmosDbBooks
{
    public class TestCh16CosmosDbContext
    {
        private readonly ITestOutputHelper _output;

        public TestCh16CosmosDbContext(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestAccessCosmosEmulator()
        {
            //SETUP
            var options = this.GetCosmosDbOptions<Ch16CosmosDbContext>();
            using var context = new Ch16CosmosDbContext(options);

            //ATTEMPT
            context.Database.EnsureCreated();

            //VERIFY
        }

        [Fact]
        public async Task TestWriteItemGuidKeyAuto()
        {
            //SETUP
            var options = this.GetCosmosDbOptions<Ch16CosmosDbContext>();
            using var context = new Ch16CosmosDbContext(options);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            //ATTEMPT
            var item1 = new CosmosGuidKey();
            var item2 = new CosmosGuidKey();
            context.AddRange(item1, item2);
            await context.SaveChangesAsync();

            //VERIFY
            context.ChangeTracker.Clear();
            var readBook = await context.GuidKeyItems.ToListAsync();
            readBook.Count.ShouldEqual(2);
        }

        [Fact]
        public async Task TestFilterByDate()
        {
            //SETUP
            var options = this.GetCosmosDbOptions<Ch16CosmosDbContext>();
            using var context = new Ch16CosmosDbContext(options);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var item1 = new CosmosGuidKey{MyDateTime = new DateTime(2000,1,1)};
            var item2 = new CosmosGuidKey{MyDateTime = new DateTime(3000, 1, 1)};
            context.AddRange(item1, item2);
            await context.SaveChangesAsync();

            //ATTEMPT
            context.ChangeTracker.Clear();
            var readBook = await context.GuidKeyItems.Where(x => x.MyDateTime < new DateTime(2010,1,1) ).ToListAsync();

            //VERIFY
            readBook.Count.ShouldEqual(1);
        }

        [Fact]
        public async Task TestFilterByDateYear()
        {
            //SETUP
            var options = this.GetCosmosDbOptions<Ch16CosmosDbContext>();
            using var context = new Ch16CosmosDbContext(options);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var item1 = new CosmosGuidKey
            {
                MyDateTime = new DateTime(2000, 1, 1),
                MyTimSpan = new TimeSpan(1,2,3,4)
            };
            var item2 = new CosmosGuidKey
            {
                MyDateTime = new DateTime(3000, 1, 1),
                MyTimSpan = new TimeSpan(4,3,2,1 )
            };
            context.AddRange(item1, item2);
            await context.SaveChangesAsync();

            //ATTEMPT
            context.ChangeTracker.Clear();
            var readBook = await context.GuidKeyItems.Where(x => x.MyDateTime < new DateTime(2100, 1, 1)).ToListAsync();

            //VERIFY
            readBook.Count.ShouldEqual(1);
        }

        [Fact]
        public async Task TestWriteItemCompositeKeyAuto()
        {
            //SETUP
            var options = this.GetCosmosDbOptions<Ch16CosmosDbContext>();
            using var context = new Ch16CosmosDbContext(options);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            //ATTEMPT
            var item1 = new CosmosCompositeKey {Key1 = Guid.NewGuid(), Key2 = 1};
            var item2 = new CosmosCompositeKey {Key1 = Guid.NewGuid(), Key2 = 2};
            context.AddRange(item1, item2);
            await context.SaveChangesAsync();

            //VERIFY
            context.ChangeTracker.Clear();
            var readBook = await context.ComKeyItems.ToListAsync();
            readBook.Count.ShouldEqual(2);
        }

        [RunnableInDebugOnly]
        public async Task TestReadAlteredDatabase()
        {
            //SETUP
            var options = this.GetCosmosDbOptions<Ch16CosmosDbContext>();
            using var context = new Ch16CosmosDbContext(options);

            //ATTEMPT
            var readBook = await context.GuidKeyItems.ToListAsync();

            //VERIFY
            readBook.Count.ShouldEqual(2);
        }

    }
}