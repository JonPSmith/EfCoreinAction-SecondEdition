// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Test.Chapter10Listings.EfClasses;
using Test.Chapter10Listings.EfCode;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using TestSupportSchema;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch10_Sequence
    {

        [Fact]
        public void TestSequenceAddOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureClean();

                //ATTEMPT
                var entity = new Order();
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                entity.OrderNo.ShouldNotEqual(0);
            }
        }

        [Fact]
        public void TestSequenceAddTwiceOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter10DbContext>();
            using (var context = new Chapter10DbContext(options))
            {
                context.Database.EnsureClean();

                //ATTEMPT
                var entity1 = new Order();
                var entity2 = new Order();
                context.AddRange(entity1, entity2);
                context.SaveChanges();

                //VERIFY
                entity2.OrderNo.ShouldEqual(entity1.OrderNo + 5);
            }
        }

    }
}