// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch11_ContextModel
    {
        private readonly ITestOutputHelper _output;

        public Ch11_ContextModel(ITestOutputHelper output)
        {
            _output = output;
        }

        public string BuildDeleteEntitySql<TEntity>                         //#A
            (DbContext context, string foreignKeyName)                      //#A
            where TEntity : class
        {
            var entityType = context.Model.FindEntityType(typeof(TEntity)); //#B

            var fkProperty = entityType?.GetForeignKeys()                   //#C
                .SingleOrDefault(x => x.Properties.Count == 1               //#C
                    && x.Properties.Single().Name == foreignKeyName)        //#C
                ?.Properties.Single();                                      //#C

            if (fkProperty == null)                                         //#D
                throw new ArgumentException($"Something wrong!");           //#D

            var fullTableName = entityType.GetSchema() == null              //#E
                ? entityType.GetTableName()                                 //#E
                : $"{entityType.GetSchema()}.{entityType.GetTableName()}";  //#E

            return $"DELETE FROM {fullTableName} " +                        //#F
                   $"WHERE {fkProperty.GetColumnName()}"                    //#F
                   + " = {0}";                                              //#G
        }
        /********************************************************************
        #A This method provides a quick way to delete all the entities linked to a principal entity
        #B This gets the Model information for the given type - null if not there
        #C This looks for a foreign key with a single property with the given name
        #D If any of those things don't work it throws an exception
        #E The forms the full table name, with a schema if required.
        #F This forms the main part of the SQL code
        #G This adds a parameter so that the ExecuteSqlRaw can check that parameter
         **********************************************************/


        [Fact]
        public void TestBuildDeleteEntitySqlStringOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var sql = BuildDeleteEntitySql<Review>(context, nameof(Review.BookId));

                //VERIFY
                sql.ShouldEqual("DELETE FROM Review WHERE BookId = {0}");
            }
        }

        [Fact]
        public void TestBuildDeleteEntitySqlApplyToDatabaseOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var books = context.SeedDatabaseFourBooks();

                //ATTEMPT
                var sql = BuildDeleteEntitySql<Review>(context, nameof(Review.BookId));
                var numDeleted = context.Database.ExecuteSqlRaw(sql, books.Last().BookId);

                //VERIFY
                numDeleted.ShouldEqual(2);
                context.Set<Review>().Count().ShouldEqual(0);
            }
        }

        [Fact]
        public void TestDeleteReviewsUsingEfCoreOk()
        {
            //SETUP
            var showLog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<EfCoreContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.ToString());
            });
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var books = context.SeedDatabaseFourBooks();

                //ATTEMPT
                showLog = true;
                context.RemoveRange(books.Last().Reviews);
                context.SaveChanges();

                //VERIFY

                context.Set<Review>().Count().ShouldEqual(0);
            }
        }

    }
}
