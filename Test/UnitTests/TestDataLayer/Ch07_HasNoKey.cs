// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter07Listings;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch07_HasNoKey
    {
        private readonly ITestOutputHelper _output;

        public Ch07_HasNoKey(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestToViewQueryOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureCreated();

                context.Add(new BaseClass() { MyInt = 123, MyString = "Hello"});
                context.SaveChanges();

                //ATTEMPT
                var query = context.DupClasses;
                var entities = query.ToList();

                //VERIFY
                _output.WriteLine(query.ToQueryString());
                entities.Single().MyInt.ShouldEqual(123);
                entities.Single().MyString.ShouldEqual("Hello");
            }
        }

        [Fact]
        public void TestHasNoKeyQueryOk()
        {
            //SETUP
            var sharedConnection = new SqliteSharedConnection();
            using (var context = new Chapter07DbContext(sharedConnection.GetOptions<Chapter07DbContext>()))
            {
                context.Database.EnsureCreated();

                context.Add(new BaseClass() {MyInt = 123, MyString = "Hello"});
                context.SaveChanges();
            }
            using (var context = new DupDbContext(sharedConnection.GetOptions<DupDbContext>()))
            {
                //ATTEMPT
                var query = context.DupClasses;
                var entities = query.ToList();

                //VERIFY
                _output.WriteLine(query.ToQueryString());
                entities.Single().MyInt.ShouldEqual(123);
                entities.Single().MyString.ShouldEqual("Hello");
            }
        }

        [Fact]
        public void TestToViewAttemptToWriteThrowsException()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                context.Add(new DupClass() { MyInt = 123, MyString = "Hello" });
                var ex = Assert.Throws<InvalidOperationException>(() => context.SaveChanges());

                //VERIFY
                ex.Message.ShouldStartWith("The entity type 'DupClass' is not mapped to a table, therefore the entities cannot be persisted to the database.");
            }
        }

        [Fact]
        public void TestHasNoKeyAttemptToWriteThrowsException()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<DupDbContext>();
            using (var context = new DupDbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() =>
                    context.Add(new DupClass() { MyInt = 123, MyString = "Hello" }));

                //VERIFY
                ex.Message.ShouldStartWith("Unable to track an instance of type 'DupClass' because it does not have a primary key.");
            }
        }

        private class DupDbContext : DbContext
        {
            public DupDbContext(
                DbContextOptions<DupDbContext> options)
                : base(options)
            { }

            public DbSet<IndexClass> IndexClasses { get; set; }

            public DbSet<ValueConversionExample> ConversionExamples { get; set; }

            public DbSet<BaseClass> BaseClasses { get; set; }
            public DbSet<DupClass> DupClasses { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<DupClass>()
                    .ToTable("KeyTestTable")
                    .HasNoKey();
            }
        }
    }
}