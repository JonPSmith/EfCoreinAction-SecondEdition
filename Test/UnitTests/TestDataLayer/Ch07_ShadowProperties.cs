// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter07Listings;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch07_ShadowProperties
    {
        [Fact]
        public void ReadShadowPropertyAsNoTrackedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    var timeNow = DateTime.Now;
                    var entity = new MyEntityClass
                    { NormalProp = "Hello" };
                    context.Add(entity);
                    context.Entry(entity).Property("UpdatedOn").CurrentValue = timeNow;
                    context.SaveChanges();

                    //VERIFY
                    var readEntity = context.MyEntities.AsNoTracking().First();
                    context.Entry(readEntity).Property("UpdatedOn").CurrentValue.ShouldEqual(new DateTime());
                }
            }
        }

        [Fact]
        public void ReadShadowPropertyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    var timeNow = DateTime.Now;
                    var entity = new MyEntityClass
                    { NormalProp = "Hello" };
                    context.Add(entity); 
                    context.Entry(entity).Property("UpdatedOn").CurrentValue = timeNow; 
                    context.SaveChanges(); 

                    //VERIFY
                    var shadowPropUpdatedOn = context.MyEntities.Select(b => EF.Property<DateTime>(b, "UpdatedOn")).First();
                    shadowPropUpdatedOn.ShouldEqual(timeNow);
                }
            }
        }

        [Fact]
        public void SetShadowPropertyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    var entity = new MyEntityClass  //#A
                        { NormalProp = "Hello"};//#A
                    context.Add(entity); //#B
                    context.Entry(entity) //#C
                        .Property("UpdatedOn").CurrentValue //#D
                            = DateTime.Now; //#E
                    context.SaveChanges(); //#F
                    /************************************************
                    #A I create an entity class
                    #B ... and add it to the context. That means it is now tracked
                    #C I then get the EntityEntry from the tracked entity data
                    #D Using the Property method I can get the shadow property with read/write access
                    #E Then I set that property to the value I want
                    #F Finally I call SaveChanges to save the MyEntityClass instance, with its normal and shadow property values, to the database
                     * *********************************************/
                    //VERIFY
                }
            }
        }
        //private readonly ITestOutputHelper _output;

        //public Ch07_ShadowProperties(ITestOutputHelper output)
        //{
        //    _output = output;
        //}

        [Fact]
        public void TestShadowPropertyExistsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                {
                    //ATTEMPT

                    //VERIFY
                    context.GetColumnName<MyEntityClass>("UpdatedOn").ShouldEqual("UpdatedOn");
                }
            }
        }
    }
}