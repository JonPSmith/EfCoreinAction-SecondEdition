// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter08Listings.PropertyBags;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch08_PropertyBags
    {
        private ITestOutputHelper _output;

        public Ch08_PropertyBags(ITestOutputHelper output)
        {
            _output = output;
        }
        
        public TableSpec DefineTable(string primaryKeyName = "Id")
        {
            return new TableSpec("Table1",
                new List<PropertySpec>
                {
                    new PropertySpec(primaryKeyName, typeof(int)),
                    new PropertySpec("Title", typeof(string), true),
                    new PropertySpec("Price", typeof(double)),
                }
            );
        }

        [Fact]
        public void TestCreateReadPropertyBagsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<PropertyBagsDbContext>();
            using (var context = new PropertyBagsDbContext(options, DefineTable()))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var propBag = new Dictionary<string, object> //#A
                {
                    ["Title"] = "My book",  //#B
                    ["Price"] = 123.0       //#B
                };
                context.MyTable.Add(propBag); //#C
                context.SaveChanges();       //#D

                var readInPropBag = context.MyTable //#E
                    .Single(x => (int) x["Id"] == 1); //#F

                //VERIFY
                readInPropBag["Id"].ShouldNotEqual(0);          //#G
                readInPropBag["Title"].ShouldEqual("My book");  //#G
                readInPropBag["Price"].ShouldEqual(123.0);      //#G
                /******************************************************************
                #A The property bags type is of type Dictionary<string, object>
                #B To set the various properties using the normal dictionary approaches
                #C For shared types, which property bags are, you must provide the DbSet to Add to
                #D Now that property bags entry is saved in the normal way
                #E To read back you use the DbSet mapped to the property bags entity
                #F To refer to a property/column you need to use an indexer. You may need to cast the object to the right type
                #G You access the result using normal dictionary access methods
                 ************************************************************/

                var viewResults = context.TestAccess.ToList();
                viewResults.Count().ShouldEqual(1);
                viewResults.Single().Id.ShouldNotEqual(0);
                viewResults.Single().Title.ShouldEqual("My book");
                viewResults.Single().Price.ShouldEqual(123.0);
            }
        }

        [Fact]
        public void TestCreatePropertyBagsLeaveOutPropertyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<PropertyBagsDbContext>();
            using (var context = new PropertyBagsDbContext(options, DefineTable()))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var propBag = new Dictionary<string, object> //#A
                {
                    ["Title"] = "My book",

                };
                context.MyTable.Add(propBag); 
                context.SaveChanges();

                //VERIFY
                var viewResults = context.TestAccess.ToList();
                viewResults.Count().ShouldEqual(1);
                viewResults.Single().Id.ShouldNotEqual(0);
                viewResults.Single().Title.ShouldEqual("My book");
                viewResults.Single().Price.ShouldEqual(0.0);
            }
        }

        [Fact]
        public void TestCreatePropertyBagsCheckTitleIsRequiredOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<PropertyBagsDbContext>();
            using (var context = new PropertyBagsDbContext(options, DefineTable()))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var propBag = new Dictionary<string, object> //#A
                {
                    ["Price"] = 123.0,

                };
                context.MyTable.Add(propBag);
                var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.ShouldEqual("SQLite Error 19: 'NOT NULL constraint failed: Table1.Title'.");
            }
        }

        //NOTE: you need to run this separately because EF Core caches the setup
        [RunnableInDebugOnly]
        public void TestCreatePropertyBagsBadPrimaryKeyNameOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<PropertyBagsDbContext>();
            using (var context = new PropertyBagsDbContext(options, DefineTable("badName")))
            {
                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => context.Database.EnsureCreated());

                //VERIFY
                ex.Message.ShouldEqual("The entity type 'Table1' requires a primary key to be defined. If you intended to use a keyless entity type call 'HasNoKey()'.");
            }
        }

    }
}