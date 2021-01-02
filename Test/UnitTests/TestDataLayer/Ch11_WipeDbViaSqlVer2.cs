// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

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
    public class Ch11_WipeDbViaSqlVer2
    {
        private readonly ITestOutputHelper _output;

        public Ch11_WipeDbViaSqlVer2(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void GetTableNameEfCoreContextOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var eType = context.Model
                    .FindEntityType(typeof(Book).FullName);
                var bookTableName = eType.GetTableName();

                //VERIFY
                bookTableName.ShouldEqual("Books");
            }
        }

        [Fact]
        public void OutputAllRelationshipsEfCoreContextOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                ListToEntitiesWithRelationships(context);
            }
        }

        private void ListToEntitiesWithRelationships(DbContext context)
        {
            var allEntities = context.Model.GetEntityTypes().ToList();
            foreach (var entity in allEntities)
            {
                _output.WriteLine($"{entity}");
                var fKeys = entity.GetForeignKeys().ToList();
                if (fKeys.Any())
                {
                    _output.WriteLine("      Principals are:");
                    foreach (var fKey in fKeys)
                    {
                        _output.WriteLine($"           {fKey.PrincipalEntityType}");
                    }
                }
            }
        }

        [Fact]
        public void GetTableNamesInOrderToDeleteOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {


                //ATTEMPT
                var tableNames = string.Join(",", context.GetTableNamesInOrderForWipe());

                //VERIFY
                tableNames.ShouldEqual("[BookTag],[BookAuthor],[LineItem],[PriceOffers],[Review],[Orders],[Authors],[Tags],[Books]");

            }
        }

        [Fact]
        public void WipeAllTablesEfCoreContextOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            context.WipeAllDataFromDatabase();
            context.ChangeTracker.Clear();

            //VERIFY
            context.Books.Count().ShouldEqual(0);
            context.Authors.Count().ShouldEqual(0);
        }

    }
}
