// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Dapper;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.Chapter11Listings.Dtos;
using Test.Chapter11Listings.EfCode;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch11_RawSqlCommands
    {
        private readonly ITestOutputHelper _output;

        public Ch11_RawSqlCommands(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestFromSqlOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureClean();
                context.AddUpdateSqlProcs();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                int filterBy = 5;
                var books = context.Books //#A
                    .FromSqlInterpolated( //#B
                        $"EXECUTE dbo.FilterOnReviewRank @RankFilter = {filterBy}") //#C
                    .IgnoreQueryFilters() //#D
                    .ToList();

                /***********************************************************
                #A I start the query in the normal way, with the DbSet<T> I want to read
                #B The FromSqlInterpolated method then allows me to insert a SQL command. 
                #C This uses C#6's string interpolation feature to provide the parameter
                #D You need to remove any query filters otherwise the SQL isn't valid
                 * ********************************************************/

                //VERIFY
                books.Count.ShouldEqual(1);
                books.First().Title.ShouldEqual("Quantum Networking");
            }
        }

        [Fact]
        public void TestFromSqlWithIncludeOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureClean();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                double minStars = 4;
                var books = context.Books
                    .FromSqlRaw(
                       "SELECT * FROM Books b WHERE " +                //#A
                         "(SELECT AVG(CAST([NumStars] AS float)) " +   //#A
                         "FROM dbo.Review AS r " +                     //#A
                         "WHERE b.BookId = r.BookId) >= {0}", minStars)//#B
                    .Include(r => r.Reviews)                           //#C
                    .AsNoTracking()                                    //#D
                    .ToList();

                /**************************************************************
                #A The SQL calculates the average votes and uses it in a SQL WHERE 
                #B In this case I use the normal sql parameter check and substitution method of {0}, {2}, {3} etc.
                #C The Include method works with the FromSql because I am not executing a store procedure
                #D You can add other EF Core commands after the SQL command 
                 * ****************************************************************/

                //VERIFY
                books.Count.ShouldEqual(1);
                books.First().Title.ShouldEqual("Quantum Networking");
                books.First().Reviews.Count().ShouldEqual(2);
            }
        }

        [Fact]
        public void TestFromSqlWithOrderBad()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureClean();
                context.SeedDatabaseFourBooks();
                
                //ATTEMPT
                var ex = Assert.Throws<Microsoft.Data.SqlClient.SqlException>(
                    () => context.Books
                        .FromSqlRaw(
                            "SELECT * FROM Books AS a ORDER BY PublishedOn DESC")
                        .ToList());

                //VERIFY
                ex.Message.ShouldEqual("The ORDER BY clause is invalid in views, inline functions, derived tables, subqueries, and common table expressions, unless TOP, OFFSET or FOR XML is also specified.");
            }
        }

        [Fact]
        public void TestFromSqlQueryFiltersHasEffectOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureClean();
                var fourBooks = EfTestData.CreateFourBooks();
                fourBooks[1].SoftDeleted = true;
                context.AddRange(fourBooks);
                context.SaveChanges();

                //ATTEMPT
                var query = context.Books
                    .FromSqlRaw("SELECT * FROM Books")
                    .IgnoreQueryFilters();
                var books = query.ToList();        

                //VERIFY
                _output.WriteLine(query.ToQueryString());
                books.Count.ShouldEqual(4);
            }
        }

        [Fact]
        public void TestFromSqlRawWithSelectDoesNotReduceDatabaseAccesesOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureClean();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var query = context.Books
                    .FromSqlRaw("SELECT * FROM Books")
                    .Select(x => new { x.BookId, x.Title});
                var books = query.ToList();

                //VERIFY
                _output.WriteLine(query.ToQueryString());
                query.ToQueryString().ShouldContain("SELECT * FROM Books");
                books.Count.ShouldEqual(4);
            }
        }

        [Fact]
        public void TestFromSqlWithOrderAndIgnoreQueryFiltersOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureClean();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var query =
                    context.Books
                        .FromSqlRaw(
                            "SELECT * FROM Books " +
                            "WHERE SoftDeleted = 0 " + //#B
                            "ORDER BY PublishedOn DESC")
                        .IgnoreQueryFilters(); //#C
                var books = query.ToList();

                /************************************************************
                #A You have to remove the effect of a model-level query filter in certain SQL commands such as ORDER BY as they won't work
                #B I add the model-query filter code back in by hand
                #C It is the ORDER BY in this case that cannot be run with a model-level query filter 
                 * ********************************************************/
                //VERIFY
                books.First().Title.ShouldEqual("Quantum Networking");
                _output.WriteLine(query.ToQueryString());
            }
        }

        [Fact]
        public void TestSqlViaDapperOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureClean();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var connection = context.Database.GetDbConnection();           //#A
                string query = "SELECT b.BookId, b.Title, " +                  //#B
                               "(SELECT AVG(CAST([NumStars] AS float)) " +     //#B
                               "FROM dbo.Review AS r " +                       //#B
                               "WHERE b.BookId = r.BookId) AS AverageVotes " + //#B
                               "FROM Books b " +
                               "WHERE b.BookId = @bookId";                     //#B

                var bookDto = connection                                      //#C
                    .Query<RawSqlDto>(query, new                               //#C
                    {                                                          //#D
                        bookId = 4                                             //#D
                    })                                                         //#D
                    .Single();

                /****************************************************************
                #A Gets a DbConnection to the database, which the micro-ORM called Dapper can use 
                #B Create the SQL query you want to execute
                #C Call Dapper's Query method with the type of the returned data
                #D You provide parameters to Dapper to be added to the SQL command 
                 * ******************************************************************/

                //VERIFY
                bookDto.AverageVotes.ShouldEqual(5);
            }
        }

        [Fact]
        public void TestSqlToNonEntityClassOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureClean();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var bookDtos = new List<RawSqlDto>();
                var conn = context.Database.GetDbConnection(); //#A
                try
                {
                    conn.Open(); //#B
                    using (var command = conn.CreateCommand())//#C
                    {
                        string query = "SELECT b.BookId, b.Title, " + //#D
                        "(SELECT AVG(CAST([NumStars] AS float)) " + //#D
                        "FROM dbo.Review AS r " +                   //#D
                            "WHERE b.BookId = r.BookId) AS AverageVotes " + //#D
                            "FROM Books b"; //#D
                        command.CommandText = query; //#E

                        using (DbDataReader reader = command.ExecuteReader()) //F
                        {
                            while (reader.Read()) //#G
                            {
                                var row = new RawSqlDto
                                {
                                    BookId = reader.GetInt32(0), //#H
                                    Title = reader.GetString(1), //#H
                                    AverageVotes = reader.IsDBNull(2) 
                                        ? null : (double?) reader.GetDouble(2) //#H
                                };
                                bookDtos.Add(row);
                            }
                        }
                    }
                }
                finally
                {
                    conn.Close(); //#I
                }
                /****************************************************************
                #A I ask EF Core for a DbConnection, which the low-level SqlClient library can use 
                #B I need to open the connection before I use it
                #C I create a DbCommand on that connection
                #D This library transfers SQL directly to the database server, hence all the database accesses have to be defined in SQL
                #E I assign my command to the DbCommand instance
                #F The ExecuteReader method sends the SQL command to the database server and then creates a reader to read the data that the server will return
                #G This tries to reaad the next row and returns true if it was successful
                #H I have to hand-code the conversion and copying of the data from the reader into my class
                #I When the read has finished I need to close the connection to the database server
                 * ******************************************************************/


                //VERIFY
                bookDtos.Count.ShouldEqual(4);
                bookDtos.First().AverageVotes.ShouldBeNull();
                bookDtos.Last().AverageVotes.ShouldEqual(5);
            }
        }

        [Fact]
        public void TestExecuteSqlCommandOk()
        {
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureClean();
                context.SeedDatabaseFourBooks();

                var bookId = context.Books.
                    Single(x => x.Title == "Quantum Networking").BookId;
                var uniqueString = Guid.NewGuid().ToString();

                //ATTEMPT
                var rowsAffected = context.Database //#A
                    .ExecuteSqlRaw(                 //#B
                        "UPDATE Books " +           //#C
                        "SET Description = {0} " +  //#C
                        "WHERE BookId = {1}",       //#C
                        uniqueString, bookId);      //#D
                /*********************************************************
                #A The ExecuteSqlRaw can be found in the context.Database property
                #B The ExecuteSqlRaw will execute the SQL and return an integer, which in this case is the number of rows updated
                #C The SQL command as a string, with places for the parameters to be inserted.
                #D I provide two parameters which referred to in the command
                 * **********************************************************/

                //VERIFY
                rowsAffected.ShouldEqual(1);
                context.Books.AsNoTracking().Single(x => x.BookId == bookId).Description.ShouldEqual(uniqueString);
            }
        }

        [Fact]
        public void TestReloadOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureClean();
                context.SeedDatabaseFourBooks();

                var entity = context.Books.            //#A
                    Single(x => x.Title == "Quantum Networking");
                var uniqueString = Guid.NewGuid().ToString();

                context.Database.ExecuteSqlRaw(        //#B
                        "UPDATE Books " +              //#B
                        "SET Description = {0} " +     //#B
                        "WHERE BookId = {1}",          //#B
                        uniqueString, entity.BookId);  //#B

                //ATTEMPT
                context.Entry(entity).Reload();        //#C

                /*************************************************
                #A I load a Book entity in the normal way
                #B I now use ExecuteSqlRaw to change the Description column of that same Book entity.
                #C By calling the Reload method EF Core will reread that entity to make sure the local copy is up to date.
                 * **************************************************/
                
                //VERIFY
                entity.Description.ShouldEqual(uniqueString);
            }
        }

        [Fact]
        public void TestReloadWithChangeOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureClean();
                context.SeedDatabaseFourBooks();

                var entity = context.Books.
                    Single(x => x.Title == "Quantum Networking");
                var uniqueString = Guid.NewGuid().ToString();

                var rowsAffected = context.Database 
                    .ExecuteSqlRaw( 
                        "UPDATE Books " + 
                        "SET Description = {0} " +
                        "WHERE BookId = {1}",
                        uniqueString, entity.BookId); 

                //ATTEMPT
                entity.Title = "Changed it";
                context.Entry(entity).Reload();

                //VERIFY
                entity.Description.ShouldEqual(uniqueString);
                entity.Title.ShouldEqual("Quantum Networking");
            }
        }

        [Fact]
        public void TestReloadWithNavigationalChangeOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureClean();
                context.SeedDatabaseFourBooks();

                var entity = context.Books
                    .Include(x => x.Reviews)
                    .Single(x => x.Title == "Quantum Networking");
                var uniqueString = Guid.NewGuid().ToString();

                var rowsAffected = context.Database
                    .ExecuteSqlRaw(
                        "UPDATE Books " +
                        "SET Description = {0} " +
                        "WHERE BookId = {1}",
                        uniqueString, entity.BookId);

                //ATTEMPT
                entity.Reviews.Add(new Review());
                context.Entry(entity).Reload();

                //VERIFY
                entity.Description.ShouldEqual(uniqueString);
                entity.Reviews.Count().ShouldEqual(3);
            }
        }

        [Fact]
        public void TestGetDatabaseValuesOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureClean();
                context.SeedDatabaseFourBooks();

                var entity = context.Books.
                    Single(x => x.Title == "Quantum Networking");
                var uniqueString = Guid.NewGuid().ToString();

                var rowsAffected = context.Database
                    .ExecuteSqlRaw(
                        "UPDATE Books " +
                        "SET Description = {0} " +
                        "WHERE BookId = {1}",
                        uniqueString, entity.BookId);

                //ATTEMPT
                var values = context.Entry(entity).GetDatabaseValues();
                var book = (Book)values.ToObject();

                //VERIFY
                book.Description.ShouldEqual(uniqueString);
            }
        }


    }
}