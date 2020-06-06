// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter07Listings;
using TestSupport.EfHelpers;
using TestSupportSchema;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch07_ValueConverters
    {
        private readonly ITestOutputHelper _output;

        public Ch07_ValueConverters(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestValueConvertersSqlServerOk()
        {
            //SETUP
            var utcTime = new DateTime(1000, DateTimeKind.Utc);
            var showLog = false;
            var options = this.CreateUniqueClassOptionsWithLogging<Chapter07DbContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.ToString());
            });
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureClean();

                var entity = new ValueConversionExample
                {
                    Stage = Stages.Two,
                    StageViaAttribute = Stages.Two,
                    StageViaFluent = Stages.Two,
                    StageCanBeNull = null,
                    EnumFlags = FlagEnum.Flag1 | FlagEnum.Flag3,
                    DateTimeUtc = utcTime,
                    DateTimeUtcUtcOnReturn = utcTime,
                    DateTimeUtcSaveAsString = utcTime
                };
                showLog = true;
                context.Add(entity);
                context.SaveChanges();
                showLog = false;
            }
            using (var context = new Chapter07DbContext(options))
            {

                //ATTEMPT
                var entityFromDb = context.ConversionExamples.Single();

                //VERIFY
                entityFromDb.Stage.ShouldEqual(Stages.Two);
                entityFromDb.StageViaAttribute.ShouldEqual(Stages.Two);
                entityFromDb.StageViaFluent.ShouldEqual(Stages.Two);
                entityFromDb.EnumFlags.ShouldEqual(FlagEnum.Flag1 | FlagEnum.Flag3);
                entityFromDb.DateTimeUtc.Kind.ShouldEqual(DateTimeKind.Unspecified);
                entityFromDb.DateTimeUtcUtcOnReturn.Kind.ShouldEqual(DateTimeKind.Utc);
                entityFromDb.DateTimeUtcSaveAsString.Kind.ShouldEqual(DateTimeKind.Unspecified);
            }
        }

        [Fact]
        public void TestValueConvertersSqliteOk()
        {
            //SETUP
            var utcTime = new DateTime(1000, DateTimeKind.Utc);
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new ValueConversionExample
                {
                    Stage = Stages.Two,
                    StageViaAttribute = Stages.Two,
                    StageViaFluent = Stages.Two,
                    EnumFlags = FlagEnum.Flag1 | FlagEnum.Flag3,
                    DateTimeUtc = utcTime,
                    DateTimeUtcUtcOnReturn = utcTime,
                    DateTimeUtcSaveAsString = utcTime
                };
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter07DbContext(options))
            {

                //ATTEMPT
                var entityFromDb = context.ConversionExamples.Single();

                //VERIFY
                entityFromDb.Stage.ShouldEqual(Stages.Two);
                entityFromDb.StageViaAttribute.ShouldEqual(Stages.Two);
                entityFromDb.StageViaFluent.ShouldEqual(Stages.Two);
                entityFromDb.EnumFlags.ShouldEqual(FlagEnum.Flag1 | FlagEnum.Flag3);
                entityFromDb.DateTimeUtc.Kind.ShouldEqual(DateTimeKind.Unspecified);
                entityFromDb.DateTimeUtcUtcOnReturn.Kind.ShouldEqual(DateTimeKind.Utc);
                entityFromDb.DateTimeUtcSaveAsString.Kind.ShouldEqual(DateTimeKind.Unspecified);
            }
        }

        [Fact]
        public void TestValueConvertersCurrentValuesOk()
        {
            //SETUP
            var utcTime = new DateTime(1000, DateTimeKind.Utc);
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new ValueConversionExample
                {
                    Stage = Stages.Two,
                    StageViaAttribute = Stages.Two,
                    StageViaFluent = Stages.Two,
                    EnumFlags = FlagEnum.Flag1 | FlagEnum.Flag3,
                };
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter07DbContext(options))
            {

                //ATTEMPT
                var entry = context.Entry(context.ConversionExamples.Single());

                //VERIFY
                entry.Property(nameof(ValueConversionExample.Stage)).CurrentValue.ShouldEqual(Stages.Two);
                entry.Property(nameof(ValueConversionExample.StageViaAttribute)).CurrentValue.ShouldEqual(Stages.Two);
                entry.Property(nameof(ValueConversionExample.StageViaFluent)).CurrentValue.ShouldEqual(Stages.Two);
                entry.Property(nameof(ValueConversionExample.EnumFlags)).CurrentValue.ShouldEqual(FlagEnum.Flag1 | FlagEnum.Flag3);
            }
        }

        [Fact]
        public void TestValueConvertersSortOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureCreated();

                context.Add(new ValueConversionExample { StageViaFluent = Stages.Three });
                context.Add(new ValueConversionExample{ StageViaFluent = Stages.One});
                context.Add(new ValueConversionExample { StageViaFluent = Stages.None });
                context.Add(new ValueConversionExample { StageViaFluent = Stages.Two });
                context.SaveChanges();
            }
            using (var context = new Chapter07DbContext(options))
            {
                //ATTEMPT
                var query = context.ConversionExamples
                    .OrderBy(x => x.StageViaFluent);
                var entities = query.ToList();

                //VERIFY
                _output.WriteLine(query.ToQueryString());
                entities.Select(x => x.StageViaFluent).ShouldEqual(
                    new List<Stages> {Stages.None, Stages.One, Stages.Three, Stages.Two});
            }
        }

        [Fact]
        public void TestValueConvertersFilterOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter07DbContext>();
            using (var context = new Chapter07DbContext(options))
            {
                context.Database.EnsureCreated();

                context.Add(new ValueConversionExample { StageViaFluent = Stages.Three });
                context.Add(new ValueConversionExample { StageViaFluent = Stages.One });
                context.Add(new ValueConversionExample { StageViaFluent = Stages.None });
                context.Add(new ValueConversionExample { StageViaFluent = Stages.Two });
                context.SaveChanges();
            }
            using (var context = new Chapter07DbContext(options))
            {
                //ATTEMPT
                var query = context.ConversionExamples
                    .Where(x => x.StageViaFluent == Stages.One || x.StageViaFluent == Stages.Two);
                var entities = query.ToList();

                //VERIFY
                _output.WriteLine(query.ToQueryString());
                entities.Select(x => x.StageViaFluent).ShouldEqual(
                    new List<Stages> { Stages.One, Stages.Two });
            }
        }
    }
}