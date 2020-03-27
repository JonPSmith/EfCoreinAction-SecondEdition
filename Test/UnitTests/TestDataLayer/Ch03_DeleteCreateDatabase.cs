// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Diagnostics;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch03_DeleteCreateDatabase
    {
        private readonly ITestOutputHelper _output;

        public Ch03_DeleteCreateDatabase(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestCreateDeleteDatabaseOk()
        {
            //SETUP
            var sw = new Stopwatch();
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                sw.Start();
                context.Database.EnsureDeleted();
                sw.Stop();
                var deleteTime = sw.ElapsedMilliseconds;
                sw.Start();
                context.Database.EnsureCreated();
                sw.Stop();
                var createTime = sw.ElapsedMilliseconds;
                _output.WriteLine("It took {0:,} ms to delete, and {1:,} ms to create", deleteTime, createTime);
            }
        }

        
    }
}