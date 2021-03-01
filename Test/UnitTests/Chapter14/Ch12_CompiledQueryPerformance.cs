// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BookApp.Domain.Books;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.Chapter14
{
    public class Ch12_CompiledQueryPerformance
    {
        private readonly ITestOutputHelper _output;

        public Ch12_CompiledQueryPerformance(ITestOutputHelper output)
        {
            _output = output;
        }

        private static Func<BookDbContext, int, Book> _compiledQueryComplex =
            EF.CompileQuery((BookDbContext context, int i) =>
                context.Books
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .Include(x => x.Reviews)
                    .Where(x => !x.SoftDeleted)
                    .OrderBy(x => x.PublishedOn)
                    .Skip(i)
                    .FirstOrDefault());

        private void RunNonCompiledQueryComplex(BookDbContext context, int i)
        {
            var books = context.Books
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Include(x => x.Reviews)
                .Where(x => !x.SoftDeleted)
                .OrderBy(x => x.PublishedOn)
                .Skip(i)
                .FirstOrDefault();
        }

        private static Func<BookDbContext, int, Book> //#A
            _compliedQuerySimple =                    //#A
            EF.CompileQuery( //#B
                (BookDbContext context, int i) => //#B
                context.Books//#D
                    .Skip(i) //#D
                    .First() //#D
                );
        /*************************************************
        #A You need to define a static function to hold your complied query. In this case I take in the application's DbContext, an int parameter, and the return type
        #B The EF.CompileQuery expects: a) a DbContext, b) one or two parameters for you to use in your query, c) the returned result, either a entity class or IEnumerable<TEntity>
        #C Now you define the query to want to hold as compiled
         * *************************************************/

        private void RunNonCompiledQuerySimple(BookDbContext context, int i)
        {
            var book = context.Books.Skip(i)
                .First();
        }

        private static Func<BookDbContext, IEnumerable<Book>> _compliedQueryEnumerable =
            EF.CompileQuery((BookDbContext context) =>
                context.Books);

        [RunnableInDebugOnly]
        public void QueryNonCompiledComplex()
        {
            var options = SqliteInMemory.CreateOptions<BookDbContext>();

            //ATTEMPT
            using (var context = new BookDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(100);

                //ATTEMPT
                RunTest(context, 1, "First access, NonCompiled:", RunNonCompiledQueryComplex);
                RunTest(context, 1, "Second access, NonCompiled:", RunNonCompiledQueryComplex);
                RunTest(context, 100, "Multi access, NonCompiled:", RunNonCompiledQueryComplex);
            }
        }

        [RunnableInDebugOnly]
        public void QueryCompiledComplex()
        {
            var options = SqliteInMemory.CreateOptions<BookDbContext>();

            //ATTEMPT
            using (var context = new BookDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(100);

                //ATTEMPT
                RunTest(context, 1, "First access, Compiled:", (c, i) => _compiledQueryComplex(c, i));
                RunTest(context, 1, "Second access, Compiled:", (c, i) => _compiledQueryComplex(c, i));
                RunTest(context, 100, "Multi access, Compiled:", (c, i) => _compiledQueryComplex(c, i));
            }
        }

        [RunnableInDebugOnly]
        public void QueryNonCompiledSimple()
        {
            var options = SqliteInMemory.CreateOptions<BookDbContext>();

            //ATTEMPT
            using (var context = new BookDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(100);

                //ATTEMPT
                RunTest(context, 1, "First access, NonCompiled:", RunNonCompiledQuerySimple);
                RunTest(context, 1, "Second access, NonCompiled:", RunNonCompiledQuerySimple);
                RunTest(context, 100, "Multi access, NonCompiled:", RunNonCompiledQuerySimple);
            }
        }

        [RunnableInDebugOnly]
        public void QueryCompiledSimple()
        {
            var options = SqliteInMemory.CreateOptions<BookDbContext>();

            //ATTEMPT
            using (var context = new BookDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(100);

                //ATTEMPT
                RunTest(context, 1, "First access, Compiled:", (c, i) => _compliedQuerySimple(c, i));
                RunTest(context, 1, "Second access, Compiled:", (c, i) => _compliedQuerySimple(c, i));
                RunTest(context, 100, "Multi access, Compiled:", (c, i) => _compliedQuerySimple(c, i));
            }
        }

        [Fact]
        public void TestEnumerableCompiledQuery()
        {
            var options = SqliteInMemory.CreateOptions<BookDbContext>();

            //ATTEMPT
            using (var context = new BookDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks();

                //ATTEMPT
                var result = _compliedQueryEnumerable(context);
                var list = result.ToList();

                //VERIFY
                list.Count.ShouldEqual(10);
            }
        }

        //--------------------------------------------------------

        private void RunTest(BookDbContext context, int numCyclesToRun, string testType, Action<BookDbContext, int> actionToRun)
        {
            var timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < numCyclesToRun; i++)
            {
                actionToRun(context, i);
            }
            timer.Stop();
            _output.WriteLine("Ran {0}: total time = {1} ms ({2:f1} ms per action)", testType,
                timer.ElapsedMilliseconds,
                timer.ElapsedMilliseconds / ((double)numCyclesToRun));
        }

    }
}