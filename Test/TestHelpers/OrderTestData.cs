// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using BookApp.Domain.Orders;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.Persistence.EfCoreSql.Orders;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;

namespace Test.TestHelpers
{
    public static class OrderTestData
    {
        /// <summary>
        /// This adds the BookDbContext tables to the OrderDbContext - that allows the sharing of the Book entity
        /// </summary>
        /// <param name="orderContext"></param>
        public static void BookContextEnsureCreatedOnOrderDb(this OrderDbContext orderContext)
        {
            if (!orderContext.Database.IsSqlite())
                throw new NotSupportedException("This only works on SQLite databases");

            var logs = new List<string>();
            var bookOptions = SqliteInMemory.CreateOptionsWithLogTo<BookDbContext>(log => logs.Add(log));
            using var tempBookContext = new BookDbContext(bookOptions);
            tempBookContext.Database.EnsureCreated();

            foreach (var log in logs)
            {
                var index = log.IndexOf('\r');
                if (index < 1) continue;
                var sqlCommand = log.Substring(index + 2);
                orderContext.Database.ExecuteSqlRaw(sqlCommand);
            }
        }

        /// <summary>
        /// This adds the four tests books to the OrderDbContext to give you access to BookView entity used in the Order
        /// </summary>
        /// <param name="orderContext"></param>
        /// <returns></returns>
        public static IEnumerable<BookView> SeedFourBookDdPartWithOptionalDbSchemaAdd(this OrderDbContext orderContext, bool ensureCreated)
        {
            if (!orderContext.Database.IsSqlite())
                throw new NotSupportedException("This only works on SQLite databases");

            if (ensureCreated)
                orderContext.BookContextEnsureCreatedOnOrderDb();

            var options = SqliteInMemory.CreateOptions<BookDbContext>(
                builder => builder.UseSqlite(orderContext.Database.GetDbConnection()));
            options.StopNextDispose();
            using var bookContext = new BookDbContext(options);
            var books = bookContext.SeedDatabaseFourBooks();
            return books.Select(x => orderContext.BookViews.Single(y => y.BookId == x.BookId));
        }
    }
}