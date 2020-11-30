// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using Test.Chapter14Listings;
using TestSupport.EfHelpers;
using TestSupportSchema;
using Xunit;
using Xunit.Abstractions;

namespace Test.UnitTests.Chapter14
{
    public class TestDoNotCallSaveChangesMultipleTimes
    {
        private readonly ITestOutputHelper _output;

        public TestDoNotCallSaveChangesMultipleTimes(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestIndividualSaveChanges()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter14DbContext>();
            using var context = new Chapter14DbContext(options);
            context.Database.EnsureClean();

            //ATTEMPT
            using (new TimeThings(_output, "individual"))
            {
                for (int i = 0; i < 100; i++)
                {
                    context.Add(new MyEntity());
                    context.SaveChanges();
                }
            }
            using (new TimeThings(_output, "individual"))
            {
                for (int i = 0; i < 100; i++)
                {
                    context.Add(new MyEntity());
                    context.SaveChanges();
                }
            }
            using (new TimeThings(_output, "individual"))
            {               
                for (int i = 0; i < 100; i++)
                {
                    context.Add(new MyEntity());
                    context.SaveChanges();
                }
            }

            //VERIFY
        }

        [Fact]
        public void TestOneSaveChanges()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter14DbContext>();
            using var context = new Chapter14DbContext(options);
            context.Database.EnsureCreated();

            context.WipeAllDataFromDatabase();

            //ATTEMPT
            using (new TimeThings(_output, "One SaveChanges"))
            {
                for (int i = 0; i < 100; i++)
                {
                    context.Add(new MyEntity());
                }
                context.SaveChanges();
            }
            using (new TimeThings(_output, "One SaveChanges"))
            {
                for (int i = 0; i < 100; i++)
                {
                    context.Add(new MyEntity());
                }
                context.SaveChanges();
            }
            using (new TimeThings(_output, "One SaveChanges"))
            {
                for (int i = 0; i < 100; i++)
                {
                    context.Add(new MyEntity());
                }
                context.SaveChanges();
            }

            //VERIFY
        }

        [Fact]
        public void TestOneAddAndSaveChanges()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter14DbContext>();
            using var context = new Chapter14DbContext(options);
            context.Database.EnsureCreated();

            context.WipeAllDataFromDatabase();

            //ATTEMPT
            using (new TimeThings(_output, "One Add and one SaveChanges"))
            {
                var list = new List<MyEntity>();
                for (int i = 0; i < 100; i++)
                {
                    list.Add(new MyEntity());
                }
                context.AddRange(list);
                context.SaveChanges();
            }
            using (new TimeThings(_output, "One Add and one SaveChanges"))
            {
                var list = new List<MyEntity>();
                for (int i = 0; i < 100; i++)
                {
                    list.Add(new MyEntity());
                }
                context.AddRange(list);
                context.SaveChanges();
            }
            using (new TimeThings(_output, "One Add and one SaveChanges"))
            {
                var list = new List<MyEntity>();
                for (int i = 0; i < 100; i++)
                {
                    list.Add(new MyEntity());
                }
                context.AddRange(list);
                context.SaveChanges();
            }

            //VERIFY
        }
    }
}