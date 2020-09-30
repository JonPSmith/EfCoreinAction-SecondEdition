// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Test.Chapter08Listings.PropertyBags;
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
        
        public TableSpec DefineTable()
        {
            return new TableSpec("Table1",
                new List<PropertySpec>
                {
                    new PropertySpec("Id", typeof(int)),
                    new PropertySpec("Title", typeof(string), true),
                    new PropertySpec("Price", typeof(double)),
                }
            );
        }

        [Fact]
        public void TestCreateBookWithTagsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<PropertyBagsDbContext>();
            var tableSpec = DefineTable();
            using (var context = new PropertyBagsDbContext(options, tableSpec))
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


    }
}