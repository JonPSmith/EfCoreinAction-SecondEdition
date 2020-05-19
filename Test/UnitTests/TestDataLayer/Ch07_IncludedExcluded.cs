// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Test.Chapter07Listings;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch07_IncludedExcluded
    {
        //private readonly ITestOutputHelper _output;

        //public Ch07_ShadowProperties(ITestOutputHelper output)
        //{
        //    _output = output;
        //}

        [Fact]
        public void TestIncludedPropertiesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                {
                    //ATTEMPT
                    var props = context.GetProperties<MyEntityClass>().Select(x => x.Name).ToList();

                    //VERIFY
                    props.ShouldEqual(new List<string>{ "MyEntityClassId", "InternalSet", "NormalProp", "PrivateSet", "UpdatedOn", "ReadOnlyIntMapped" });
                }
            }
        }
    }
}