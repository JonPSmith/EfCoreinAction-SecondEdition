// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter08Listings.EFCode;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using TestSupportSchema;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch08_CheckSqlServer
    {
        public Ch08_CheckSqlServer(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;

        [RunnableInDebugOnly]
        public void TestCreateSplitOwnDbContextSqlServerDbOk()
        {
            //SETUP
            var options = this.CreateUniqueMethodOptionsWithLogging<SplitOwnDbContext>(log => _output.WriteLine(log.Message));
            using (var context = new SplitOwnDbContext(options))
            {
                //ATTEMPT
                context.Database.EnsureClean();

                //VERIFY
            }
        }

        [RunnableInDebugOnly]
        public void TestDeleteDependentDefaultOk()
        {
            //SETUP

            var showLog = false;
            var options = this.CreateUniqueClassOptionsWithLogging<Chapter08DbContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.ToString());
            });
            using (var context = new Chapter08DbContext(options))
            {

                context.Database.EnsureClean();

                var numPrincipal = context.DeletePrincipals.Count();
                var numDependent = context.Set<DeleteDependentDefault>().Count();

                var entity = new DeletePrincipal {DependentDefault = new DeleteDependentDefault()};
                context.Add(entity);
                context.SaveChanges();

                //ATTEMPT
                showLog = true;
                context.Remove(entity);
                context.SaveChanges();
                showLog = false;

                //VERIFY
                entity.DependentDefault.DeletePrincipalId.ShouldBeNull();
                context.DeletePrincipals.Count().ShouldEqual(numPrincipal);
                context.Set<DeleteDependentDefault>().Count().ShouldEqual(numDependent+1);
            }
        }

        [Fact]
        public void TestCreateChapter07DbContextSqlServerOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                //ATTEMPT
                context.Database.EnsureClean();

                //VERIFY
            }
        }
    }
}