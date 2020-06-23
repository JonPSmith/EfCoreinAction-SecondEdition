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
        public void TestMoveColumnsSqlServerOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<MoveColumnsDbContext>();
            using (var context = new MoveColumnsDbContext(options))
            {
                context.Database.EnsureDeleted();

                //ATTEMPT
                context.Database.Migrate();

                //VERIFY
            }
        }

    }
}