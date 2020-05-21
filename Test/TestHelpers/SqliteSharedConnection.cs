// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Test.TestHelpers
{
    public class SqliteSharedConnection : IDisposable
    {
        private readonly DbConnection _connection;

        public SqliteSharedConnection()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();
        }

        public DbContextOptions<T> GetOptions<T>() where T : DbContext
        {
            return new DbContextOptionsBuilder<T>()
                .UseSqlite(_connection).Options;
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}