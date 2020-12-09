// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit.Abstractions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch05_AsyncPerformance
    {
        private readonly ITestOutputHelper _output;

        private readonly DbContextOptions<EfCoreContext> _options;

        private readonly int _firstBookId;

        public Ch05_AsyncPerformance(ITestOutputHelper output)
        {
            _output = output;

            _options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(_options))
            {
                context.Database.EnsureClean();
                context.Books.AddRange(EfTestData.CreateDummyBooks(1000, false, false));
                context.SaveChanges();
                _firstBookId = context.Books.First().BookId;
            }
        }

        [RunnableInDebugOnly]
        public async Task SimpleAssessesOk()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                //ATTEMPT
                RunTest(context, 1, "First access, synch:", (c, id) => c.Books.Single(x => x.BookId == id));
                await Task.WhenAll(RunTestAsync(context, 1, "First access, async:", (c, id) => c.Books.SingleAsync(x => x.BookId == id)));

                await Task.WhenAll(RunTestAsync(context, 1, "1 access, async:", (c, id) => c.Books.SingleAsync(x => x.BookId == id)));
                await Task.WhenAll(RunTestAsync(context, 100, "100 access, async:", (c, id) => c.Books.SingleAsync(x => x.BookId == id)));
                RunTest(context, 1, "1 access, synch:", (c, id) => c.Books.Single(x => x.BookId == id));
                RunTest(context, 100, "100 access, synch:", (c, id) => c.Books.Single(x => x.BookId == id));
                await Task.WhenAll(RunTestAsync(context, 100, "100 access, async:", (c, id) => c.Books.SingleAsync(x => x.BookId == id)));
                RunTest(context, 100, "100 access, synch:", (c, id) => c.Books.Single(x => x.BookId == id));
            }
            //VERIFY
        }

        [RunnableInDebugOnly]
        public async Task MediumAssessesOk()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                //ATTEMPT
                RunTest(context, 1, "1 access, synch:", (c, id) => c.Books
                    .Include(x => x.AuthorsLink)
                    .ThenInclude(x => x.Author)
                    .Include(x => x.Reviews)
                    .Include(x => x.Promotion)
                    .Single(x => x.BookId == id));
                await Task.WhenAll(RunTestAsync(context, 1, "1 access, async:", (c, id) => c.Books
                    .Include(x => x.AuthorsLink)
                    .ThenInclude(x => x.Author)
                    .Include(x => x.Reviews)
                    .Include(x => x.Promotion)
                    .SingleAsync(x => x.BookId == id)));
                await Task.WhenAll(RunTestAsync(context, 100, "100 access, async:", (c, id) => c.Books
                    .Include(x => x.AuthorsLink)
                    .ThenInclude(x => x.Author)
                    .Include(x => x.Reviews)
                    .Include(x => x.Promotion)
                    .SingleAsync(x => x.BookId == id)));
                RunTest(context, 100, "100 acces, synch:", (c, id) => c.Books
                    .Include(x => x.AuthorsLink)
                    .ThenInclude(x => x.Author)
                    .Include(x => x.Reviews)
                    .Include(x => x.Promotion)
                    .Single(x => x.BookId == id));
                await Task.WhenAll(RunTestAsync(context, 100, "100 access, async:", (c, id) => c.Books
                    .Include(x => x.AuthorsLink)
                    .ThenInclude(x => x.Author)
                    .Include(x => x.Reviews)
                    .Include(x => x.Promotion)
                    .SingleAsync(x => x.BookId == id)));
                RunTest(context, 100, "100 access, synch:", (c, id) => c.Books
                    .Include(x => x.AuthorsLink)
                    .ThenInclude(x => x.Author)
                    .Include(x => x.Reviews)
                    .Include(x => x.Promotion)
                    .Single(x => x.BookId == id));
            }
            //VERIFY
        }

        [RunnableInDebugOnly]
        public async Task ManySimpleAssessesOk()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                //ATTEMPT
                RunTest(context, 1, "First access, synch:", MultipleSmall);
                await Task.WhenAll(RunTestAsync(context, 1, "First access, async:", MultipleSmallAsync));

                //NOTE: This takes so long that I have reduced the number of times from 100 to 10 - BUT that affects the perfomance figures!!
                await RunTestAsync(context, 1, "1 access, async:", MultipleSmallAsync);
                await RunTestAsync(context, 10, "10 access, async:", MultipleSmallAsync);
                RunTest(context, 10, "10 access, synch:", MultipleSmall);
                await RunTestAsync(context, 10, "10 access, async:", MultipleSmallAsync);
                RunTest(context, 10, "10 access, synch:", MultipleSmall);
                await RunTestAsync(context, 10, "10 access, async:", MultipleSmallAsync);
                RunTest(context, 10, "10 access, synch:", MultipleSmall);
            }
            //VERIFY
        }


        [RunnableInDebugOnly]
        public async Task ComplexAssessesOk()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                //ATTEMPT
                RunTest(context, 1, "First access, synch:", (c, id) => c.Books.Where(x => x.Reviews.Count > 3).OrderByDescending(x => x.Price).Take(10).ToList());
                await Task.WhenAll(RunTestAsync(context, 1, "First access, async:", (c, id) => c.Books.Where(x => x.Reviews.Count > 3).OrderByDescending(x => x.Price).Take(10).ToListAsync()));

                await Task.WhenAll(RunTestAsync(context, 1, "1 access, async:", (c, id) => c.Books.Where(x => x.Reviews.Count > 3).OrderByDescending(x => x.Price).Take(10).ToListAsync()));
                await Task.WhenAll(RunTestAsync(context, 100, "100 access, async:", (c, id) => c.Books.Where(x => x.Reviews.Count > 3).OrderByDescending(x => x.Price).Take(10).ToListAsync()));
                RunTest(context, 1, "1 access, synch:", (c, id) => c.Books.Where(x => x.Reviews.Count > 3).OrderByDescending(x => x.Price).Take(10).ToList());
                RunTest(context, 100, "100 access, synch:", (c, id) => c.Books.Where(x => x.Reviews.Count > 3).OrderByDescending(x => x.Price).Take(10).ToList());
                await Task.WhenAll(RunTestAsync(context, 100, "100 access, async:", (c, id) => c.Books.Where(x => x.Reviews.Count > 3).OrderByDescending(x => x.Price).Take(10).ToListAsync()));
                RunTest(context, 100, "100 access, synch:", (c, id) => c.Books.Where(x => x.Reviews.Count > 3).OrderByDescending(x => x.Price).Take(10).ToList());
            }
        }
        
        //--------------------------------------------------------

        private void RunTest(EfCoreContext context, int numCyclesToRun, string testType, Action<EfCoreContext, int> actionToRun)
        {
            var timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < numCyclesToRun; i++)
            {
                actionToRun(context, i + _firstBookId);
            }
            timer.Stop();
            _output.WriteLine("Ran {0}: total time = {1} ms ({2:f2} ms per action)", testType,
                timer.ElapsedMilliseconds,
                timer.ElapsedMilliseconds / ((double) numCyclesToRun));
        }

        private async Task RunTestAsync(EfCoreContext context, int numCyclesToRun, string testType, Func<EfCoreContext, int, Task> actionToRun)
        {
            var timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < numCyclesToRun; i++)
            {
                await actionToRun(context, i + _firstBookId);//.ConfigureAwait(false);
            }
            timer.Stop();
            _output.WriteLine("Ran {0}: total time = {1} ms ({2:f2} ms per action)", testType,
                timer.ElapsedMilliseconds,
                timer.ElapsedMilliseconds / ((double)numCyclesToRun));
        }

        private void MultipleSmall(EfCoreContext context, int id)
        {
            var book = context.Books.Single(x => x.BookId == id);
            context.Entry(book).Collection(c => c.AuthorsLink).Load();
            foreach (var authorLink in book.AuthorsLink)
            {                                        
                context.Entry(authorLink)            
                    .Reference(r => r.Author).Load();
            }
            context.Entry(book).Collection(c => c.Reviews).Load();
            context.Entry(book).Reference(r => r.Promotion).Load();
        }

        private async Task MultipleSmallAsync(EfCoreContext context, int id)
        {
            var book = await context.Books.SingleAsync(x => x.BookId == id);
            await context.Entry(book).Collection(c => c.AuthorsLink).LoadAsync();
            foreach (var authorLink in book.AuthorsLink)
            {
                await context.Entry(authorLink)
                    .Reference(r => r.Author).LoadAsync();
            }
            await context.Entry(book).Collection(c => c.Reviews).LoadAsync();
            await context.Entry(book).Reference(r => r.Promotion).LoadAsync();
        }
    }
}