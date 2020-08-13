// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using BookApp.Persistence.NormalSql.Books;
using BookApp.Persistence.NormalSql.Orders;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;

namespace Test.TestHelpers
{
    public static class OrderTestData
    {
        public static void BookContextEnsureCreatedOnOrderDb(this OrderDbContext orderContext)
        {
            if (!orderContext.Database.IsSqlite())
                throw new NotSupportedException("This only works on SQLite databases");

            var logs = new List<string>();
            var bookOptions = SqliteInMemory.CreateOptionsWithLogging<BookDbContext>(log => logs.Add(log.DecodeMessage()));
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


        public static IEnumerable<int> SeedFourBookDdPart(this OrderDbContext orderContext)
        {
            if (!orderContext.Database.IsSqlite())
                throw new NotSupportedException("This only works on SQLite databases");

            orderContext.BookContextEnsureCreatedOnOrderDb();

            var options = SqliteInMemory.CreateOptions<BookDbContext>(
                builder => builder.UseSqlite(orderContext.Database.GetDbConnection()));
            using var bookContext = new BookDbContext(options);
            var books = bookContext.SeedDatabaseFourBooks();
            return books.Select(x => x.BookId);
        }
    }
}