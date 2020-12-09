// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Test.Mocks;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch04_SaveChangesWithValidation
    {

        [Fact]
        public void LocalValidateOk()
        {
            //SETUP
            var lineItem = new LineItem
                    {
                        ChosenBook = new Book { PublishedOn = new DateTime(2000,1,1)},
                        LineNum = 1,
                        BookPrice = 123,
                        NumBooks = 1

            };
            //ATTEMPT
            var context = new ValidationContext(lineItem);
            var errors = new List<ValidationResult>();
            var isOk = Validator.TryValidateObject(lineItem, context, errors, true);

            //VERIFY
            isOk.ShouldBeTrue();
            errors.Any().ShouldBeFalse();
        }

        [Fact]
        public void LocalValidateLineNumBad()
        {
            //SETUP
            var lineItem = new LineItem
            {
                BookId = 1,
                LineNum = 100,
                BookPrice = 123,
                NumBooks = 1

            };
            //ATTEMPT
            var context = new ValidationContext(lineItem);
            var errors = new List<ValidationResult>();
            var isOk = Validator.TryValidateObject(lineItem, context, errors, true);

            //VERIFY
            isOk.ShouldBeFalse();
            errors.Any().ShouldBeTrue();
            errors.First().ErrorMessage.ShouldEqual("This order is over the limit of 5 books.");
        }

        [Fact]
        public void LocalValidateNumBooksBad()
        {
            //SETUP
            var lineItem = new LineItem
            {
                ChosenBook = new Book { PublishedOn = new DateTime(2000, 1, 1) },
                LineNum = 1,
                BookPrice = 123,
                NumBooks = 200

            };
            //ATTEMPT
            var context = new ValidationContext(lineItem);
            var errors = new List<ValidationResult>();
            var isOk = Validator.TryValidateObject(lineItem, context, errors, true);

            //VERIFY
            isOk.ShouldBeFalse();
            errors.Any().ShouldBeTrue();
            errors.First().ErrorMessage.ShouldEqual("If you want to order a 100 or more books please phone us on 01234-5678-90");
        }


        [Fact]
        public void ValidateOk()
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options, new FakeUserIdService(userId));
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var order = new Order
            {
                CustomerId = userId,
                LineItems = new List<LineItem>
                {
                    new LineItem
                    {
                        BookId = context.Books.First().BookId,
                        LineNum = 1,
                        BookPrice = 123,
                        NumBooks = 1
                    }
                }
            };
            context.Orders.Add(order);
            var errors = context.SaveChangesWithValidation();

            //VERIFY
            errors.Any().ShouldBeFalse();
            context.Orders.Count().ShouldEqual(1);
        }


        [Fact]
        public void ValidateLineNumNotSetBad()
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options, new FakeUserIdService(userId));
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            var order = new Order
            {
                CustomerId = userId,
                LineItems = new List<LineItem>
                {
                    new LineItem
                    {
                        BookId = context.Books.First().BookId,
                        LineNum = 20,
                        BookPrice = 123,
                        NumBooks = 1
                    }
                }
            };
            context.Orders.Add(order);
            var errors = context.SaveChangesWithValidation();

            //VERIFY
            errors.Count.ShouldEqual(1);
            errors.First().ErrorMessage.ShouldEqual("This order is over the limit of 5 books.");
            context.Orders.Count().ShouldEqual(0);
        }

    }
}