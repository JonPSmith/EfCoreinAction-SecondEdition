// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using BookApp.Infrastructure.Books.Seeding;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.Persistence.EfCoreSql.Orders;
using EfSchemaCompare;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestBookAppUi
{
    public class TestCompareSchema
    {

        [Fact]
        public void TestWriteBooksAsyncNoDataCausesNewDbOk()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString();
            var options1 = new DbContextOptionsBuilder<BookDbContext>().UseSqlServer(connection).Options;
            using var context1 = new BookDbContext(options1);
            var options2 = new DbContextOptionsBuilder<OrderDbContext>().UseSqlServer(connection).Options;
            using var context2 = new OrderDbContext(options2);

            context1.Database.EnsureDeleted();

            context1.Database.Migrate();
            context2.Database.Migrate();

            //ATTEMPT
            var config = new CompareEfSqlConfig();
            config.IgnoreTheseErrors(@"DIFFERENT: Author->Property 'LastUpdatedUtc', default value sql. Expected = <null>, found = '0001-01-01T00:00:00.0000000'
DIFFERENT: Author->Property 'LastUpdatedUtc', value generated. Expected = Never, found = OnAdd
DIFFERENT: Author->Property 'WhenCreatedUtc', default value sql. Expected = <null>, found = '0001-01-01T00:00:00.0000000'
DIFFERENT: Author->Property 'WhenCreatedUtc', value generated. Expected = Never, found = OnAdd
DIFFERENT: Book->Property 'AuthorsOrdered', value generated. Expected = OnUpdateSometimes, found = Never
DIFFERENT: Book->Property 'LastUpdatedUtc', default value sql. Expected = <null>, found = '0001-01-01T00:00:00.0000000'
DIFFERENT: Book->Property 'LastUpdatedUtc', value generated. Expected = Never, found = OnAdd
DIFFERENT: Book->Property 'ReviewsAverageVotes', value generated. Expected = OnUpdateSometimes, found = Never
DIFFERENT: Book->Property 'ReviewsCount', value generated. Expected = OnUpdateSometimes, found = Never
DIFFERENT: Book->Property 'WhenCreatedUtc', default value sql. Expected = <null>, found = '0001-01-01T00:00:00.0000000'
DIFFERENT: Book->Property 'WhenCreatedUtc', value generated. Expected = Never, found = OnAdd
DIFFERENT: BookAuthor->Property 'LastUpdatedUtc', default value sql. Expected = <null>, found = '0001-01-01T00:00:00.0000000'
DIFFERENT: BookAuthor->Property 'LastUpdatedUtc', value generated. Expected = Never, found = OnAdd
DIFFERENT: BookAuthor->Property 'WhenCreatedUtc', default value sql. Expected = <null>, found = '0001-01-01T00:00:00.0000000'
DIFFERENT: BookAuthor->Property 'WhenCreatedUtc', value generated. Expected = Never, found = OnAdd
DIFFERENT: BookDetails->Property 'AuthorsOrdered', value generated. Expected = OnUpdateSometimes, found = Never
DIFFERENT: BookDetails->Property 'ReviewsAverageVotes', value generated. Expected = OnUpdateSometimes, found = Never
DIFFERENT: BookDetails->Property 'ReviewsCount', value generated. Expected = OnUpdateSometimes, found = Never
DIFFERENT: Review->Property 'LastUpdatedUtc', default value sql. Expected = <null>, found = '0001-01-01T00:00:00.0000000'
DIFFERENT: Review->Property 'LastUpdatedUtc', value generated. Expected = Never, found = OnAdd
DIFFERENT: Review->Property 'WhenCreatedUtc', default value sql. Expected = <null>, found = '0001-01-01T00:00:00.0000000'
DIFFERENT: Review->Property 'WhenCreatedUtc', value generated. Expected = Never, found = OnAdd
EXTRA IN DATABASE: BookView->PrimaryKey 'PK_Books', column name. Found = BookId
DIFFERENT: BookView->Property 'ImageUrl', column type. Expected = varchar(max), found = varchar(200)
DIFFERENT: BookView->Property 'Title', nullability. Expected = NULL, found = NOT NULL
NOT IN DATABASE: LineItem->ForeignKey 'FK_LineItem__BookViewBookId', constraint name. Expected = FK_LineItem__BookViewBookId
EXTRA IN DATABASE: Index 'Books', index constraint name. Found = IX_Books_ActualPrice
EXTRA IN DATABASE: Index 'Books', index constraint name. Found = IX_Books_LastUpdatedUtc
EXTRA IN DATABASE: Index 'Books', index constraint name. Found = IX_Books_PublishedOn
EXTRA IN DATABASE: Index 'Books', index constraint name. Found = IX_Books_ReviewsAverageVotes
EXTRA IN DATABASE: Index 'Books', index constraint name. Found = IX_Books_SoftDeleted
EXTRA IN DATABASE: Index 'Books', index constraint name. Found = IX_Books_ActualPrice
EXTRA IN DATABASE: Index 'Books', index constraint name. Found = IX_Books_LastUpdatedUtc
EXTRA IN DATABASE: Index 'Books', index constraint name. Found = IX_Books_PublishedOn
EXTRA IN DATABASE: Index 'Books', index constraint name. Found = IX_Books_ReviewsAverageVotes
EXTRA IN DATABASE: Index 'Books', index constraint name. Found = IX_Books_SoftDeleted");
            var comparer = new CompareEfSql(config);

            //ATTEMPT
            //Its starts with the connection string/name  and then you can have as many contexts as you like
            var hasErrors = comparer.CompareEfWithDb(context1, context2);

            //VERIFY
            hasErrors.ShouldBeFalse(comparer.GetAllErrors);
        }
    }
}