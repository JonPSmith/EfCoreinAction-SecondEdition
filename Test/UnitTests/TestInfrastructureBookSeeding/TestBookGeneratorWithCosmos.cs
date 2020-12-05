// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Infrastructure.Books.Seeding;
using BookApp.Persistence.CosmosDb.Books;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.Extensions.DependencyInjection;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestInfrastructureBookSeeding
{
    public class TestBookGeneratorWithCosmos : IDisposable
    {
        private readonly ITestOutputHelper _output;

        private readonly CosmosDbContext _cosmosContext;
        private readonly BookDbContext _sqlContext;

        private readonly IServiceProvider _serviceProvider;

        public TestBookGeneratorWithCosmos(ITestOutputHelper output)
        {
            _output = output;

            var cosmosOptions = this.GetCosmosDbOptions<CosmosDbContext>();
            _cosmosContext = new CosmosDbContext(cosmosOptions);

            var sqlOptions = this.CreateUniqueClassOptions<BookDbContext>();
            _sqlContext = new BookDbContext(sqlOptions);

            var services = new ServiceCollection();
            services.AddSingleton(sqlOptions);
            services.AddSingleton(cosmosOptions);

            _serviceProvider = services.BuildServiceProvider();
        }

        public void Dispose()
        {
            _cosmosContext?.Dispose();
            _sqlContext?.Dispose();
        }

        private async Task ResetDatabasesAsync()
        {
            await _cosmosContext.Database.EnsureDeletedAsync();
            await _cosmosContext.Database.EnsureCreatedAsync();
            _sqlContext.Database.EnsureClean();
        }


        [Fact]
        public async Task TestWriteBooksAsyncNoDataCausesNewDbOk()
        {
            //SETUP
            await ResetDatabasesAsync();

            var fileDir = Path.Combine(TestData.GetTestDataDir());
            var generator = new BookGenerator(_serviceProvider);

            //ATTEMPT
            await generator.WriteBooksAsync(fileDir, false, 1, true, default);

            //VERIFY
            var cosmosBooks = _cosmosContext.Books.ToList();
            cosmosBooks.Count().ShouldEqual(6);
            cosmosBooks.All(x => x.Tags.Select(y => y.TagId).Contains("Manning books")).ShouldBeTrue();
        }

        [Fact]
        public async Task TestWriteBooksAsyncCheckReviews()
        {
            //SETUP
            await ResetDatabasesAsync();

            var fileDir = Path.Combine(TestData.GetTestDataDir());
            var generator = new BookGenerator(_serviceProvider);

            //ATTEMPT
            await generator.WriteBooksAsync(fileDir, false, 20, true, default);

            //VERIFY
            foreach (var book in _cosmosContext.Books)
            {
                _output.WriteLine(book.ToString());
            }

            var cosmosBooks = _cosmosContext.Books.ToList();
            cosmosBooks.Count(x => x.ReviewsCount > 0).ShouldEqual(13);
            //Can't get exact review value compare
        }

    }
}