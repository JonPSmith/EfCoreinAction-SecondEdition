// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using Test.Chapter08Listings.EFCode;
using Test.Chapter08Listings.SplitOwnClasses;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch08_OwnedTypes
    {
        public Ch08_OwnedTypes(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;

        //-----------------------------------------------
        //private helper methods
        private static void AddOrderWithAddresses(SplitOwnDbContext context)
        {
            var entity = new OrderInfo()
            {
                OrderNumber = "123",
                DeliveryAddress = new Address
                {
                    NumberAndStreet = "1, some street",
                    City = "Some city",
                    ZipPostCode = "W1A 1AA",
                    CountryCodeIso2 = "UK"
                },
                BillingAddress = new Address
                {
                    NumberAndStreet = "1, some street",
                    City = "Some city",
                    ZipPostCode = "1234-5678",
                    CountryCodeIso2 = "US"
                }
            };
            context.Add(entity);
            context.SaveChanges();
        }

        private static void AddUserWithHomeAddresses(SplitOwnDbContext context)
        {
            var entity = new User()
            {
                Name = "Unit Test",
                HomeAddress = new Address
                {
                    NumberAndStreet = "1, my street",
                    City = "My city",
                    ZipPostCode = "W1A 1AA",
                    CountryCodeIso2 = "UK"
                }
            };
            context.Add(entity);
            context.SaveChanges();
        }
        //---------------------------------------------------

        [Fact]
        public void TestCreateOrderWithAddressesOk()
        {
            //SETUP
            var showLog = false;
            var options = this.CreateUniqueClassOptionsWithLogging<SplitOwnDbContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.ToString());
            });
            using (var context = new SplitOwnDbContext(options))
            {
                context.Database.EnsureClean();

                //ATTEMPT
                showLog = true;
                AddOrderWithAddresses(context);
            }
        }

        [Fact]
        public void TestReadOrderWithAddressesOk()
        {
            //SETUP
            var logToOptions = new LogToOptions
            {
                ShowLog = false
            };
            var options = SqliteInMemory.CreateOptionsWithLogTo<SplitOwnDbContext>(
                _output.WriteLine, logToOptions);
            using var context = new SplitOwnDbContext(options);
            context.Database.EnsureCreated();

            context.Database.EnsureCreated();
            AddOrderWithAddresses(context);

            context.ChangeTracker.Clear();

            //ATTEMPT
            logToOptions.ShowLog = true;
            var order = context.Orders.Single();

            //VERIFY
            order.DeliveryAddress.ShouldNotBeNull();
            order.BillingAddress.ShouldNotBeNull();
        }

        [Fact]
        public void TestCreateOrderAddressesNull()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SplitOwnDbContext>();
            using var context = new SplitOwnDbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var entity = new OrderInfo()
            {
                OrderNumber = "123"
            };
            context.Add(entity);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //VERIFY
            var order = context.Orders.Single();
            order.DeliveryAddress.ShouldBeNull();
            order.BillingAddress.ShouldBeNull();
        }

        //-------------------------------------------------
        //Owned type in separate table

        [Fact]
        public void TestCreateUserWithAddressOk()
        {
            //SETUP
            var showLog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<SplitOwnDbContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.ToString());
            });
            using var context = new SplitOwnDbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            showLog = true;
            AddUserWithHomeAddresses(context);
            showLog = false;

            context.ChangeTracker.Clear();

            //VERIFY
            context.Users.Count().ShouldEqual(1);
        }

        //SEE https://github.com/dotnet/efcore/issues/22444 BUG
        [Fact]
        public void TestCreateUserWithAddressReadBackFindOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SplitOwnDbContext>();
            using var context = new SplitOwnDbContext(options);
            context.Database.EnsureCreated();

            AddUserWithHomeAddresses(context);
            
            context.ChangeTracker.Clear();

            //ATTEMPT
            var user = context.Find<User>(1);

            //VERIFY
            user.HomeAddress.ShouldNotBeNull();
        }

        [Fact]
        public void TestCreateUserWithAddressReadBackNoIncludeOk()
        {
            //SETUP
            var showLog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<SplitOwnDbContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.ToString());
            });
            using var context = new SplitOwnDbContext(options);
            context.Database.EnsureCreated();

            AddUserWithHomeAddresses(context);

            context.ChangeTracker.Clear();

            //ATTEMPT
            showLog = true;
            var user = context.Users.First();

            //VERIFY
            user.HomeAddress.ShouldNotBeNull();
        }

        [Fact]
        public void TestCreateUserWithoutAddressOk()
        {
            //SETUP
            var showLog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<SplitOwnDbContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.ToString());
            });
            using (var context = new SplitOwnDbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                showLog = true;
                var user = new User {Name = "Unit Test"};
                context.Add(user);
                context.SaveChanges();
                showLog = false;

                //VERIFY
                context.Users.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestCreateUserWithoutAddressReadBackOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SplitOwnDbContext>();
            using var context = new SplitOwnDbContext(options);
            context.Database.EnsureCreated();


            context.Add(new User {Name = "Unit Test"});
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var user = context.Users.First();

            //VERIFY
            user.HomeAddress.ShouldBeNull();
        }

        [Fact]
        public void TestDeleteUserDeletesAddressOkk()
        {
            //SETUP
            var showLog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<SplitOwnDbContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.ToString());
            });
            using var context = new SplitOwnDbContext(options);
            context.Database.EnsureCreated();

            AddUserWithHomeAddresses(context);

            context.ChangeTracker.Clear();

            //ATTEMPT
            var user = context.Users.First();
            showLog = true;
            context.Remove(user);
            context.SaveChanges();

            //VERIFY
        }

        [Fact]
        public void TestReadOrderNoIncludesOk()
        {
            //SETUP
            var showLog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<SplitOwnDbContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.ToString());
            });
            using var context = new SplitOwnDbContext(options);
            context.Database.EnsureCreated();
            AddOrderWithAddresses(context);

            context.ChangeTracker.Clear();

            //ATTEMPT
            showLog = true;
            var entity = context.Orders.First();
            showLog = false;

            //VERIFY
            entity.DeliveryAddress.ShouldNotBeNull();
            entity.BillingAddress.ShouldNotBeNull();
        }

        [Fact]
        public void TestReadOrderSelectOk()
        {
            //SETUP
            var showLog = false;
            var options = this.CreateUniqueClassOptionsWithLogging<SplitOwnDbContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.ToString());
            });
            using (var context = new SplitOwnDbContext(options))
            {
                context.Database.EnsureCreated();
                AddOrderWithAddresses(context);
            }
            using (var context = new SplitOwnDbContext(options))
            {
                //ATTEMPT
                showLog = true;
                var entity = context.Orders.Select(x => new
                {
                    x.OrderInfoId,
                    x.BillingAddress.CountryCodeIso2
                }).First();
                showLog = false;

                //VERIFY
                entity.CountryCodeIso2.ShouldEqual("US");
            }
        }

        [Fact]
        public void TestUpdateOrderOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SplitOwnDbContext>();
            using var context = new SplitOwnDbContext(options);
            context.Database.EnsureCreated();
            AddOrderWithAddresses(context);

            context.ChangeTracker.Clear();

            //ATTEMPT
            var entity = context.Orders.First();
            entity.OrderNumber = "567";
            context.SaveChanges();

            //VERIFY
            context.Orders.First().OrderNumber.ShouldEqual("567");
        }
    }
}