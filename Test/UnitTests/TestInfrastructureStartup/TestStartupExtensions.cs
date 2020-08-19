// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BookApp.Infrastructure.Book.EventHandlers.AppStart;
using BookApp.Infrastructure.Startup;
using BookApp.UI.Controllers;
using Microsoft.Extensions.DependencyInjection;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestInfrastructureStartup
{
    public class TestStartupExtensions
    {
        private ITestOutputHelper _output;

        public TestStartupExtensions(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestGetDirectoryAssembliesOk()
        {
            //SETUP
            var currAssembly = Assembly.GetAssembly(typeof(HomeController));

            //ATTEMPT
            var assemblies = currAssembly.GetProjectAssemblies("BookApp");

            //VERIFY
            foreach (var name in assemblies.Select(x => x.GetName().Name).OrderBy(x => x))
            {
                _output.WriteLine(name);
            }

            assemblies.Count.ShouldBeInRange(12, 30);

        }

        [Fact]
        public void TestFindExecuteRegisterOnStartupMethodsOk()
        {
            //SETUP
            var bookAppAss = Assembly.GetAssembly(typeof(HomeController));
            var services = new ServiceCollection();
            var configuration = AppSettings.GetConfiguration();

            //ATTEMPT
            services.FindExecuteRegisterOnStartupMethods(configuration, bookAppAss);

            //VERIFY
            services.Count.ShouldBeInRange(15,100);
        }
    }
}