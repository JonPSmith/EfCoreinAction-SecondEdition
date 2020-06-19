// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.SoftDeleteServices.Concrete;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch11_CascadeSoftDelete
    {
        [Fact]
        public void TestSeedingOnMigrateSqliteOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var books = context.SeedDatabaseFourBooks();

                //ATTEMPT
                //context.CascadeSoftDelete(books.Last());

                //VERIFY
            }
        }
    }
}