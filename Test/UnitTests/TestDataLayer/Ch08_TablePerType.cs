// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter08Listings.EFCode;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using TestSupportSchema;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch08_TablePerType
    {

        [Fact]
        public void TestContainerTablesInDbOk()
        {
            //SETUP
            //var options = this.CreateUniqueClassOptions<Chapter08DbContext>();
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var tables = context.Model.GetEntityTypes().Select(x => x.GetTableName()).ToList();

                //VERIFY
                tables.ShouldContain(nameof(ShippingContainer));
                tables.ShouldContain(nameof(PlasticContainer));
            }
        }

        [Fact]
        public void TestCreateTptContainersOk()
        {
            //SETUP
            //var options = this.CreateUniqueClassOptions<Chapter08DbContext>();
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                //see https://github.com/dotnet/efcore/issues/22573
                context.Add(new ShippingContainer {StackingMax = 5, DoorType = "SingleEnd"}); 
                context.SaveChanges();
                context.Add(new PlasticContainer { Shape = Shapes.Bottle, CapacityMl = 123 });
                context.SaveChanges();

                //VERIFY
                context.Containers.Count().ShouldEqual(2);
            }
        }

        [Fact]
        public void TestPullBackOneTypeOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                context.Add(new ShippingContainer { StackingMax = 5, DoorType = "SingleEnd" });
                context.SaveChanges();
                context.Add(new PlasticContainer { Shape = Shapes.Bottle, CapacityMl = 123 });
                context.SaveChanges();

                //ATTEMPT
                var shippingContainers = context.Containers.OfType<ShippingContainer>().ToList();

                //VERIFY
                shippingContainers.Count.ShouldEqual(1);
                shippingContainers.Single().StackingMax.ShouldEqual(5);
            }
        }


    }
}