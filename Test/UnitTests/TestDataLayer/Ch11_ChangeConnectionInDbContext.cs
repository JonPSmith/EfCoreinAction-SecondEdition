// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Test.Chapter11Listings.EfClasses;
using Test.Chapter11Listings.EfCode;
using Test.Chapter11Listings.Interfaces;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch11_ChangeConnectionInDbContext
    {
        private readonly ITestOutputHelper _output;

        public Ch11_ChangeConnectionInDbContext(ITestOutputHelper output)
        {
            _output = output;
        }


        public class TestGetConnection : IGetConnection
        {
            private readonly string _connection;

            public TestGetConnection(string connection)
            {
                _connection = connection;
            }

            public string CurrentConnection()
            {
                return _connection;
            }
        }

        [Fact]
        public void TestConnectionNullOk()
        {
            //SETUP
            var noConnection = new DbContextOptionsBuilder<ChangeConnectDbContext>()
                .UseSqlServer(null).Options;
            using (var context = new ChangeConnectDbContext(noConnection, null))
            {
                //ATTEMPT
                var entities = context.Model.GetEntityTypes().ToList();

                //VERIFY
                entities.Count.ShouldEqual(1);
            }
        }

        [Fact]
        public void TestSwapDbOk()
        {
            //SETUP
            var tenant1Connection = this.GetUniqueDatabaseConnectionString("tenant1");
            var tenant2Connection = this.GetUniqueDatabaseConnectionString("tenant2");
            var noConnection = new DbContextOptionsBuilder<ChangeConnectDbContext>()
                .UseSqlServer(null).Options;

            using (var context = new ChangeConnectDbContext(noConnection, new TestGetConnection(tenant1Connection)))
            {
                context.Database.EnsureCreated();
                context.Add(new ConnectEntity {Name = "tenant1"});
                context.SaveChanges();
            }
            using (var context = new ChangeConnectDbContext(noConnection, new TestGetConnection(tenant2Connection)))
                context.Database.EnsureCreated();

            using (var context = new ChangeConnectDbContext(noConnection, new TestGetConnection(tenant1Connection)))
            {
                //ATTEMPT
                var connectEntities = context.Entities.ToList();

                //VERIFY
                connectEntities.Count.ShouldNotEqual(0);
                connectEntities.All(x => x.Name == "tenant1").ShouldBeTrue();
            }
            using (var context = new ChangeConnectDbContext(noConnection, new TestGetConnection(tenant2Connection)))
                context.Entities.Count().ShouldEqual(0);

        }
    }
}