// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Test.Chapter17Listings;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.Chapter17Tests
{
    public class TestSqliteLimitations
    {
        private readonly ITestOutputHelper _output;

        public TestSqliteLimitations(ITestOutputHelper output)
        {
            _output = output;
        }

        //see https://docs.microsoft.com/en-us/ef/core/modeling/dynamic-model
        private class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
        {
            public object Create(DbContext context, bool designTime = false)
                => context is DiffConfigDbContext dynamicContext
                    ? (context.GetType(), dynamicContext.Config, designTime)
                    : (object)context.GetType();
        }

        [Fact]
        public void TestSqlLiteAddSchemaIgnored()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptionsWithLogTo<DiffConfigDbContext>(_output.WriteLine, null,
                builder => builder.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>());

            using var context = new DiffConfigDbContext(options, DiffConfigs.AddSchema);

            //ATTEMPT
            context.Database.EnsureCreated();

            //VERIFY
        }

        [Fact]
        public void TestSqlLiteAddSequenceFAILS()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptionsWithLogTo<DiffConfigDbContext>(_output.WriteLine, null,
                builder => builder.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>());

            using var context = new DiffConfigDbContext(options, DiffConfigs.AddSequence);

            //ATTEMPT
            var ex = Assert.Throws<NotSupportedException>(() => context.Database.EnsureCreated());

            //VERIFY
        }


        [Fact]
        public void TestSqlLiteComputedColMightFAIL()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptionsWithLogTo<DiffConfigDbContext>(_output.WriteLine, null,
                builder => builder.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>());

            using var context = new DiffConfigDbContext(options, DiffConfigs.SetComputedCol);

            //ATTEMPT
            var ex = Assert.Throws<SqliteException>(() => context.Database.EnsureCreated());

            //VERIFY
            ex.Message.ShouldEqual("SQLite Error 1: 'no such column: yyyy'.");
        }

        [Fact]
        public void TestSqlLiteDefaultColValueWorks()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptionsWithLogTo<DiffConfigDbContext>(_output.WriteLine, null,
                builder => builder.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>());

            using var context = new DiffConfigDbContext(options, DiffConfigs.SetDefaultCol);

            //ATTEMPT
            context.Database.EnsureCreated();

            //VERIFY
        }


        [Fact]
        public void TestSqlLiteDefaultUserDefinedFunctionsFAILsOnRun()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptionsWithLogTo<DiffConfigDbContext>(_output.WriteLine, null,
                builder => builder.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>());

            using var context = new DiffConfigDbContext(options, DiffConfigs.Nothing);
            context.Database.EnsureCreated();

            //ATTEMPT
            var filepath = TestData.GetFilePath("AddUserDefinedFunctions.sql");
            var ex = Assert.Throws<SqliteException>(() => context.ExecuteScriptFileInTransaction(filepath));

            //VERIFY
            ex.Message.ShouldEqual("SQLite Error 1: 'near \"IF\": syntax error'.");
        }
    }
}