// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch02_StringSearch
    {
        private const string NormalTitle = "Entity Framework in Action";

        private readonly ITestOutputHelper _output;
        public Ch02_StringSearch(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestSqlServerCaseInsensitive()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();

            //ATTEMPT
            var caseSensitivity = CheckCaseSensitivity(options, "SQL Server - no collation");

            //VERIFY
            caseSensitivity.ShouldEqual(CaseSensitivity.CaseInsensitive);
        }

        [Fact]
        public void TestSqlServerWithCollationCaseInsensitive()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();

            //ATTEMPT
            var caseSensitivity = CheckCaseSensitivityWithCollation(options, "SQL_Latin1_General_CP1_CI_AS","SQL Server - default collation");

            //VERIFY
            caseSensitivity.ShouldEqual(CaseSensitivity.CaseInsensitive);
        }

        [Fact]
        public void TestSqlServerWithCollationCaseSensitive()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();

            //ATTEMPT
            var caseSensitivity = CheckCaseSensitivityWithCollation(options, "Latin1_General_CS_AS", "SQL Server - case sensitive collation");

            //VERIFY
            caseSensitivity.ShouldEqual(CaseSensitivity.CaseSensitive);
        }

        [Fact]
        public void TestSqliteDefault()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            //ATTEMPT
            var caseSensitivity = CheckCaseSensitivity(options, "SQLite");

            //VERIFY
            caseSensitivity.ShouldEqual(CaseSensitivity.MixedCaseSensitivity); ;
        }

        [Fact]
        public void TestSqliteWithNoCaseCollation()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            //ATTEMPT
            var caseSensitivity = CheckCaseSensitivityWithCollation(options, "NOCASE", "SQLite - NoCase");

            //VERIFY
            caseSensitivity.ShouldEqual(CaseSensitivity.MixedCaseSensitivity); ;
        }

        [Fact]
        public void TestSqliteWithBinaryCollation()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            //ATTEMPT
            var caseSensitivity = CheckCaseSensitivityWithCollation(options, "BINARY", "SQLite - binary");

            //VERIFY
            caseSensitivity.ShouldEqual(CaseSensitivity.MixedCaseSensitivity); ;
        }

        private enum CaseSensitivity {NotSet, CaseInsensitive, CaseSensitive, MixedCaseSensitivity }

        private CaseSensitivity CheckCaseSensitivity(DbContextOptions<EfCoreContext> options, string databaseType)
        {
            using (var context = new EfCoreContext(options))
            {
                if (SetupDatabaseTrueIfNeedsNewData(context))
                {
                    //new database created, so seed
                    var book1 = new Book {Title = NormalTitle};
                    var book2 = new Book {Title = "entity FRAMEWORK in action"};
                    context.AddRange(book1, book2);
                    context.SaveChanges();
                }

                //ATTEMPT
                var caseSensitivity = CaseSensitivity.NotSet;
                caseSensitivity = OutputResult(context.Books.Where(x => x.Title == NormalTitle), "==", databaseType, caseSensitivity);
                caseSensitivity = OutputResult(context.Books.Where(x => x.Title.Equals(NormalTitle)), "Equals", databaseType, caseSensitivity);
                caseSensitivity = OutputResult(context.Books.Where(x => x.Title.StartsWith("Entity")), "StartsWith", databaseType, caseSensitivity);
                caseSensitivity = OutputResult(context.Books.Where(x => x.Title.EndsWith("Action")), "EndsWith", databaseType, caseSensitivity);
                caseSensitivity = OutputResult(context.Books.Where(x => x.Title.Contains("Framework")), "Contains", databaseType, caseSensitivity);
                caseSensitivity = OutputResult(context.Books.Where(x => x.Title.IndexOf("Entity") == 0), "IndexOf", databaseType, caseSensitivity);
                caseSensitivity = OutputResult(context.Books.Where(x => EF.Functions.Like(x.Title, NormalTitle)), "Like", databaseType, caseSensitivity);

                return caseSensitivity;
            }
        }

        private CaseSensitivity CheckCaseSensitivityWithCollation(DbContextOptions<EfCoreContext> options, string collationName, string databaseType)
        {
            using (var context = new EfCoreContext(options))
            {
                if (SetupDatabaseTrueIfNeedsNewData(context))
                {
                    //new database created, so seed
                    var book1 = new Book { Title = NormalTitle };
                    var book2 = new Book { Title = "entity FRAMEWORK in action" };
                    context.AddRange(book1, book2);
                    context.SaveChanges();
                }

                //ATTEMPT
                var caseSensitivity = CaseSensitivity.NotSet;
                caseSensitivity = OutputResult(context.Books.Where(x =>
                    EF.Functions.Collate(x.Title, collationName) == NormalTitle), "==", databaseType, caseSensitivity);
                caseSensitivity = OutputResult(context.Books.Where(x =>
                    EF.Functions.Collate(x.Title, collationName).Equals(NormalTitle)), "Equals", databaseType, caseSensitivity);
                caseSensitivity = OutputResult(context.Books.Where(x =>
                    EF.Functions.Collate(x.Title, collationName).StartsWith("Entity")), "StartsWith", databaseType, caseSensitivity);
                caseSensitivity = OutputResult(context.Books.Where(x =>
                    EF.Functions.Collate(x.Title, collationName).EndsWith("Action")), "EndsWith", databaseType, caseSensitivity);
                caseSensitivity = OutputResult(context.Books.Where(x =>
                    EF.Functions.Collate(x.Title, collationName).Contains("Framework")), "Contains", databaseType, caseSensitivity);
                caseSensitivity = OutputResult(context.Books.Where(x =>
                    EF.Functions.Collate(x.Title, collationName).IndexOf("Entity") == 0), "IndexOf", databaseType, caseSensitivity);
                caseSensitivity = OutputResult(context.Books.Where(x =>
                    EF.Functions.Like(EF.Functions.Collate(x.Title, collationName), NormalTitle)), "Like", databaseType, caseSensitivity);

                return caseSensitivity;
            }
        }

        CaseSensitivity OutputResult(IQueryable<Book> query, string type, string databaseType,
            CaseSensitivity lastCaseSensitivity)
        {
            var foundCount = query.Count();
            var caseInsensitive = foundCount == 2;
            var result = caseInsensitive
                ? lastCaseSensitivity == CaseSensitivity.CaseSensitive
                    ?
                    CaseSensitivity.MixedCaseSensitivity
                    : CaseSensitivity.CaseInsensitive
                : lastCaseSensitivity == CaseSensitivity.CaseInsensitive
                    ? CaseSensitivity.MixedCaseSensitivity
                    : CaseSensitivity.CaseSensitive;
            var caseText = caseInsensitive ? "INsensitive" : "sensitive";
            _output.WriteLine($"{type} on {databaseType} is\t case-{caseText}. SQL created is:");
            foreach (var line in query.ToQueryString().Split('\n').Select(x => x.Trim()))
            {
                _output.WriteLine("      " + line);
            }

            return result;
        }

        private bool SetupDatabaseTrueIfNeedsNewData(DbContext context)
        {
            if (context.Database.IsSqlServer())
            {
                context.Database.EnsureClean();
                return true;
            }

            return context.Database.EnsureCreated();
        }
    }
}