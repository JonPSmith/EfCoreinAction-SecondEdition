// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.Chapter11Listings.EfClasses;
using Test.Chapter11Listings.EfCode;
using Test.TestHelpers;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch09_ChangeTrackingPerformance
    {
        private readonly ITestOutputHelper _output;

        public Ch09_ChangeTrackingPerformance(ITestOutputHelper output)
        {
            _output = output;
        }

        [RunnableInDebugOnly]
        public void TestSaveChangesPerformance1()
        {
            TestSaveChangesPerformanceBooksOk(1);
        }

        [RunnableInDebugOnly]
        public void TestSaveChangesPerformance10()
        {
            TestSaveChangesPerformanceBooksOk(10);
        }

        [RunnableInDebugOnly]
        public void TestSaveChangesPerformance100()
        {
            TestSaveChangesPerformanceBooksOk(100);
        }

        [RunnableInDebugOnly]
        public void TestSaveChangesPerformance1000()
        {
            TestSaveChangesPerformanceBooksOk(1000);
        }

        private void TestSaveChangesPerformanceBooksOk(int numBooks)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();

            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(numBooks);
            }
            using (var context = new EfCoreContext(options))
            {
                var books = context.Books
                    .Include(x => x.AuthorsLink).ThenInclude(x => x.Author)
                    .Include(x => x.Reviews)
                    .ToList();

                //ATTEMPT
                var timer = new Stopwatch();
                timer.Start();
                context.SaveChanges();
                timer.Stop();

                //VERIFY
                _output.WriteLine("#{0:####0} books: total time = {1:F2} ms ", numBooks,
                    1000.0 * timer.ElapsedTicks / Stopwatch.Frequency);
            }
        }

        [RunnableInDebugOnly]
        public void TestSaveChangesPerformanceMyEntity1()
        {
            TestSaveChangesPerformanceMyEntityOk(1);
        }

        [RunnableInDebugOnly]
        public void TestSaveChangesPerformanceMyEntity1000()
        {
            TestSaveChangesPerformanceMyEntityOk(1000);
        }

        [RunnableInDebugOnly]
        public void TestSaveChangesPerformanceMyEntity10000()
        {
            TestSaveChangesPerformanceMyEntityOk(10000);
        }

        private void TestSaveChangesPerformanceMyEntityOk(int numEntities)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

            using (var context = new Chapter11DbContext(options))
            {
                context.Database.EnsureCreated();
                var entities = new List<MyEntity>();
                for (int i = 0; i < numEntities; i++)
                {
                    var entity = new MyEntity();
                    entities.Add(entity);
                }
                context.AddRange(entities);
                context.SaveChanges();

            }
            using (var context = new Chapter11DbContext(options))
            {
                var entities = context.MyEntities.ToList();
                entities.Count.ShouldEqual(numEntities);

                //ATTEMPT
                var timer = new Stopwatch();
                timer.Start();
                context.SaveChanges();
                timer.Stop();

                //VERIFY
                _output.WriteLine("#{0:####0} entities: total time = {1:F2} ms ", numEntities,
                    1000.0 * timer.ElapsedTicks / Stopwatch.Frequency);
            }
        }

        //[Theory]
        //[InlineData(1)]
        //[InlineData(1000)]
        //[InlineData(10000)]
        //public void TestSaveChangesPerformanceNotifyEntityOk(int numEntities)
        //{
        //    //SETUP
        //    var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();

        //    using (var context = new Chapter11DbContext(options))
        //    {
        //        context.Database.EnsureCreated();
        //        var entities = new List<NotifyEntity>();
        //        for (int i = 0; i < numEntities; i++)
        //        {
        //            var entity = new NotifyEntity();
        //            entities.Add(entity);
        //        }
        //        context.AddRange(entities);
        //        context.SaveChanges();

        //    }
        //    using (var context = new Chapter11DbContext(options))
        //    {
        //        var entities = context.Notify.ToList();
        //        entities.Count.ShouldEqual(numEntities);

        //        //ATTEMPT
        //        var timer = new Stopwatch();
        //        timer.Start();
        //        context.SaveChanges();
        //        timer.Stop();

        //        //VERIFY
        //        _output.WriteLine("#{0:####0} entities: total time = {1:2} ms ", numEntities,
        //            1000.0 * timer.ElapsedTicks / Stopwatch.Frequency);
        //    }
        //}
    }
}
