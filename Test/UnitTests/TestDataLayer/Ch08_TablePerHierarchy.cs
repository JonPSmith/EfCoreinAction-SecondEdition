// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter08Listings.EFCode;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch08_TablePerHierarchy
    {
        [RunnableInDebugOnly]
        public void TestCreateTphPaymentCashSqlOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureClean();

                var payment = new PaymentCash()
                {
                    Amount = 123
                };
                context.Add(payment);
                context.SaveChanges();

                //VERIFY
                context.Payments.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestChangePaymentTypeOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using var context = new Chapter08DbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            context.Add(new PaymentCard{Amount =  12, ReceiptCode = "1234"});
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //You MUST read it untracked because of issue #7340
            var untracked = context.Payments.AsNoTracking().Single();
            //Then you need to copy ALL the information to the new TPH type, especially its primary key.
            var changed = new PaymentCash
            {
                PaymentId = untracked.PaymentId,
                Amount = untracked.Amount,
                PType = PTypes.Cash //You MUST explicitly set the discriminator
            };
            context.Update(changed);
            context.SaveChanges();

            context.ChangeTracker.Clear();
            
            //VERIFY
            var payment = context.Payments.Single();
            payment.ShouldBeType<PaymentCash>();
        }

        [Fact]
        public void TestCreateSoldItTphPaymentCashOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var sold = new SoldIt()
                {
                    WhatSold = "A hat",
                    Payment = new PaymentCash {  Amount = 12}
                };
                context.Add(sold);
                context.SaveChanges();

                //VERIFY
                context.Payments.Count().ShouldEqual(1);
                var cash = context.Payments.First() as PaymentCash;
                cash.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestCreateTphPaymentCashOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var payment = new PaymentCash()
                {
                    Amount = 123
                };
                context.Add(payment);
                context.SaveChanges();

                //VERIFY
                context.Payments.Count().ShouldEqual(1);
                var cash = context.Payments.First() as PaymentCash;
                cash.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestReadBackDifferentPaymentsUsingOfTypeOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using var context = new Chapter08DbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            context.Add(new PaymentCash());
            context.Add(new PaymentCard());
            context.Add(new PaymentCash());
            context.Add(new PaymentCash());
            context.Add(new PaymentCard());
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //VERIFY
            context.Payments.OfType<PaymentCash>().Count().ShouldEqual(3);
            context.Payments.OfType<PaymentCard>().Count().ShouldEqual(2);
        }

        [Fact]
        public void TestReadBackSoldItOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using var context = new Chapter08DbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var sold = new SoldIt()
            {
                WhatSold = "A hat",
                Payment = new PaymentCard { Amount = 12, ReceiptCode = "1234"}
            };
            context.Add(sold);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //VERIFY
            var readSold = context.SoldThings.Include(x => x.Payment).Single(p => p.PaymentId == 1);
            readSold.Payment.PType.ShouldEqual(PTypes.Card);
            readSold.Payment.ShouldBeType<PaymentCard>();
            var card = readSold.Payment as PaymentCard;
            card.ShouldNotBeNull();
            card.ReceiptCode.ShouldEqual("1234");
        }
    }
}