// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter09Listings.MoveColumns;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using TestSupportSchema;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch09_MoveColumns
    {
        private readonly ITestOutputHelper _output;

        public Ch09_MoveColumns(ITestOutputHelper output)
        {
            _output = output;
        }

        [RunnableInDebugOnly]
        public void TestMoveColumnsMigrateSqlServerOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<MoveColumnsDbContext>();
            using (var context = new MoveColumnsDbContext(options))
            {
                context.Database.Migrate();

                //ATTEMPT
                //This was used when the first migration was created. It adds data that needs copying in the second migration
                //var u1 = new User { Name = "Jill", Street = "Jill street", City = "Jill city"};
                //var u2 = new User { Name = "Jack", Street = "Jack street", City = "Jack city" };
                //context.AddRange(u1, u2);
                //context.SaveChanges();

                //VERIFY
            }
        }

    }
}