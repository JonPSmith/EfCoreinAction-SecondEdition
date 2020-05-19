// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter07Listings;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch07_Indexes
    {
        [Fact]
        public void TestChangeIndexesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new IndexClass
                {
                    IndexNonUnique = "a",
                    IndexUnique = "a"
                };
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter07DbContext(options))
            {
                //ATTEMPT
                var entity = context.IndexClasses.First();
                entity.IndexNonUnique = "b";
                entity.IndexUnique = "hello";
                context.SaveChanges();

                //VERIFY
                context.IndexClasses.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestCreateIndexesNullOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    var entity = new IndexClass
                    {
                        IndexNonUnique = null,
                        IndexUnique = null
                    };
                    context.Add(entity);
                    context.SaveChanges();

                    //VERIFY
                    context.IndexClasses.Count().ShouldEqual(1);
                }
            }
        }
        //private readonly ITestOutputHelper _output;

        //public Ch06_Chapter06DbContext(ITestOutputHelper output)
        //{
        //    _output = output;
        //}

        [Fact]
        public void TestCreateIndexesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    var entity = new IndexClass
                    {
                        IndexNonUnique = "a",
                        IndexUnique = "a"
                    };
                    context.Add(entity);
                    context.SaveChanges();

                    //VERIFY
                    context.IndexClasses.Count().ShouldEqual(1);
                }
            }
        }

        [Fact]
        public void TestCreateMulipleIndexesBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    var entity1 = new IndexClass
                    {
                        IndexNonUnique = "a",
                        IndexUnique = "a"
                    };
                    var entity2 = new IndexClass
                    {
                        IndexNonUnique = "a",
                        IndexUnique = "a"
                    };
                    context.AddRange(entity1, entity2);
                    var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                    //VERIFY
                    ex.InnerException.Message.ShouldEqual(
                        "SQLite Error 19: 'UNIQUE constraint failed: IndexClasses.IndexUnique'.");
                }
            }
        }

        [Fact]
        public void TestCreateMulipleIndexesNullOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    var entity1 = new IndexClass
                    {
                        IndexNonUnique = null,
                        IndexUnique = null
                    };
                    var entity2 = new IndexClass
                    {
                        IndexNonUnique = null,
                        IndexUnique = null
                    };
                    context.AddRange(entity1, entity2);
                    context.SaveChanges();

                    //VERIFY
                    context.IndexClasses.Count().ShouldEqual(2);
                }
            }
        }

        [Fact]
        public void TestCreateMultipleIndexesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                {
                    context.Database.EnsureCreated();

                    //ATTEMPT
                    var entity1 = new IndexClass
                    {
                        IndexNonUnique = "a",
                        IndexUnique = "a"
                    };
                    var entity2 = new IndexClass
                    {
                        IndexNonUnique = "a",
                        IndexUnique = "b"
                    };
                    context.AddRange(entity1,entity2);
                    context.SaveChanges();

                    //VERIFY
                    context.IndexClasses.Count().ShouldEqual(2);
                }
            }
        }

        [Fact]
        public void TestCreateMultipleIndexesSqlBad()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity1 = new IndexClass
                {
                    IndexNonUnique = "a",
                    IndexUnique = "a"
                };
                var entity2 = new IndexClass
                {
                    IndexNonUnique = "a",
                    IndexUnique = "a"
                };
                context.AddRange(entity1, entity2);
                var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.ShouldEqual(
                    @"Cannot insert duplicate key row in object 'dbo.IndexClasses' with unique index 'MyUniqueIndex'. The duplicate key value is (a).
The statement has been terminated.");
            }
        }
    }
}