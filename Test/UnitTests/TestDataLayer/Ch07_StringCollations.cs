// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter07Listings;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch07_StringCollations
    {
        private readonly ITestOutputHelper _output;

        public Ch07_StringCollations(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestSimpleCollationOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureClean();

                var entity = new CollationsClass("HELLO");
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter07DbContext(options))
            {
                //ATTEMPT
                var insensitive = context.Collations.Count(x => x.NormalString == "hello");
                var sensitive = context.Collations.Count(x => x.CaseSensitiveString == "hello");

                //VERIFY
                insensitive.ShouldEqual(1);
                sensitive.ShouldEqual(0);
            }
        }

        [Fact]
        public void TestOverrideCollationOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureClean();

                var entity = new CollationsClass("HELLO");
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter07DbContext(options))
            {
                //ATTEMPT
                var insensitive = context.Collations.Count(x => x.NormalString == "hello");
                var sensitiveQuery = context.Collations.Where(x => EF.Functions
                    .Collate(x.CaseSensitiveString, "SQL_Latin1_General_CP1_CI_AS") == "hello");
                var sensitive = sensitiveQuery.Count();

                //VERIFY
                _output.WriteLine(sensitiveQuery.ToQueryString());
                insensitive.ShouldEqual(1);
                sensitive.ShouldEqual(1);
            }
        }

        [Fact]
        public void TestPerformanceOk()
        {
            //SETUP
            var numEntries = 10000;
            var options = this.CreateUniqueClassOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureClean();

                for (int i = 0; i < 1000; i++)
                {
                    var entity = new CollationsClass(Guid.NewGuid().ToString());
                    context.Add(entity);
                }
                context.SaveChanges();
            }
            using (var context = new Chapter07DbContext(options))
            {
                //ATTEMPT
                for (int i = 0; i < 5; i++)
                {
                    _output.WriteLine($"Run {i}");
                    using (new TimeThings(_output, "NormalString", numEntries))
                    {
                        var insensitive = context.Collations.Count(x => x.NormalString == "hello");
                    }
                    using (new TimeThings(_output, "CaseSensitiveString", numEntries))
                    {
                        var sensitive = context.Collations.Count(x => x.CaseSensitiveString == "hello");
                    }
                    using (new TimeThings(_output, "CaseSensitiveStringWithIndex", numEntries))
                    {
                        var sensitive = context.Collations.Count(x => x.CaseSensitiveStringWithIndex == "hello");
                    }
                    using (new TimeThings(_output, "CaseSensitiveStringWithIndex - override", numEntries))
                    {
                        var sensitive = context.Collations.Count(x => EF.Functions
                            .Collate(x.CaseSensitiveString, "SQL_Latin1_General_CP1_CI_AS") == "hello");
                    }
                }
                

            }
        }

    }
}