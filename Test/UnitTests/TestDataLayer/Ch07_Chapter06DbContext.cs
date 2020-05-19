// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter07Listings;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch07_Chapter07DbContext
    {

        [Fact]
        public void TestColumnNameSqliteOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                {
                    //ATTEMPT
                    var tableName = context.GetColumnName(new MyEntityClass(), p => p.NormalProp);

                    //VERIFY
                    tableName.ShouldEqual("SqliteDatabaseCol");
                }
            }
        }

        [Fact]
        public void TestColumnTypeSqliteOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                {
                    //ATTEMPT
                    var colType = context.GetColumnStoreType(new MyEntityClass(), p => p.NormalProp);

                    //VERIFY
                    colType.ShouldEqual("TEXT");
                }
            }
        }

        [Fact]
        public void TestGetColumnTypeSqlServerOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                //ATTEMPT
                var colType = context.GetColumnStoreType(new MyEntityClass(), p => p.NormalProp);

                //VERIFY
                colType.ShouldEqual("nvarchar(max)");
            }
        }

        [Fact]
        public void TestTableNameOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                {
                    //ATTEMPT
                    var tableName = context.GetTableName<MyEntityClass>();

                    //VERIFY
                    tableName.ShouldEqual("MyTable");
                }
            }
        }

        [Fact]
        public void WriteToDatabaseOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    context.Add(new MyEntityClass{ NormalProp = "Hello"});
                    context.SaveChanges();

                    //VERIFY
                }
            }
        }
    }
}