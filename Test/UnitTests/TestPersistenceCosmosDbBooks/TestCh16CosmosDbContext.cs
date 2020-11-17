// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Test.Chapter16Listings;
using Test.TestHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestPersistenceCosmosDbBooks
{
    public class TestCh16CosmosDbContext
    {
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
            var item1  = new CosmosGuidKey();
            var item2 = new CosmosGuidKey();
            context.AddRange(item1, item2);
            await context.SaveChangesAsync();

            //VERIFY
            context.ChangeTracker.Clear();
            var readBook = await context.GuidKeyItems.ToListAsync();
            readBook.Count.ShouldEqual(2);
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

    }
}