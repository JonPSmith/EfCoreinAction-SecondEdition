// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter07Listings;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class TestPrimaryKeys
    {
        private class CompKeys
        {
            [Key]      
            [Column(Order = 0)]      
            public int BookId { get; set; }
            [Key]                     
            [Column(Order = 1)]       
            public int AuthorId { get; set; }
        }

        private class CompKeysDbContext : DbContext
        {
            public CompKeysDbContext(DbContextOptions<CompKeysDbContext> options)
                : base(options) { }
            
            public DbSet<CompKeys> CompKeys { get; set; }
        }

        [Fact]
        public void TestCompKeysDbContextOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CompKeysDbContext>();
            using var context = new CompKeysDbContext(options);
            
            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => context.Database.EnsureCreated());
            
            //VERIFY
            ex.Message.ShouldEqual("The entity type 'CompKeys' has multiple properties with the [Key] attribute. Composite primary keys can only be set using 'HasKey' in 'OnModelCreating'.");
        }

        private class UsingKeyAttribute
        {
            [Key]
            public int PrimaryKey { get; set; }
            public string MyString { get; set; }
        }

        private class PrimaryKeysDbContext : DbContext
        {
            public PrimaryKeysDbContext(DbContextOptions<PrimaryKeysDbContext> options)
                : base(options) { }

            public DbSet<UsingKeyAttribute> KeyAttributes { get; set; }
        }

        [Fact]
        public void TestUsingKeyAttributeOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<PrimaryKeysDbContext>();
            using var context = new PrimaryKeysDbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var entity = new UsingKeyAttribute {MyString = "Test"};
            context.Add(entity);
            context.SaveChanges();
            
            //VERIFY
            entity.PrimaryKey.ShouldNotEqual(0);
        }
    }
}