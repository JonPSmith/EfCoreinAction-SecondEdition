// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch09_HandCodingChangeScripts
    {
        private readonly ITestOutputHelper _output;

        public Ch09_HandCodingChangeScripts(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CaptureSqlCodeUsedToCreateDatabase()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString(); //#A
            var optionsBuilder = new DbContextOptionsBuilder<EfCoreContext>();
            optionsBuilder.LogTo(_output.WriteLine,  //#B
                minimumLevel: LogLevel.Information); //#C
            optionsBuilder.UseSqlServer(connection); //#D
            using (var context = new EfCoreContext(optionsBuilder.Options))
            {
                context.Database.EnsureDeleted(); //#E

                //ATTEMPT
                context.Database.EnsureCreated(); //#F

                //VERIFY
            }
        }
        /***********************************************************
        #A You provide a connection string to a tests database that you can delete and recreate
        #B The LogTo method will output to xUnit's ITestOutputHelper output
        #C By default LogTo outputs all logs, but you only need the Information logs
        #D Here you define what sort of database provider you are using
        #E You must delete any existing database so the the EnsureCreated method will run
        #F EnsureCreated will create the database using the current Model held in the context
         *****************************************************************/
    }
}