// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Test")]

namespace BookApp.Infrastructure.Startup
{
    public static class StartupExtensions
    {
        public static IServiceCollection FindExecuteRegisterOnStartupMethods(
            this IServiceCollection services, IConfiguration configuration, Assembly startAssembly = null)
        {
            startAssembly ??= Assembly.GetCallingAssembly();
            var namespacePrefix = startAssembly.GetName().Name;
            namespacePrefix = namespacePrefix.Substring(0, namespacePrefix.IndexOf('.'));
            var allProjectAssembles = startAssembly.GetDirectoryAssemblies(namespacePrefix);
            foreach (var assembly in allProjectAssembles)
            {
                foreach (var type in assembly.GetExportedTypes()
                    .Where(x => x.IsClass && !x.IsAbstract && typeof(IRegisterOnStartup).IsAssignableFrom(x)))
                {
                    var method = type.GetMethod(nameof(IRegisterOnStartup.RegisterServices));
                    if (method != null)
                    {
                        object classInstance = Activator.CreateInstance(type, BindingFlags.Public | BindingFlags.Instance, null, null, null);
                        method.Invoke(classInstance, new object[] {services, configuration});
                    }
                }
            }

            return services;
        }

        internal static List<Assembly> GetDirectoryAssemblies(this Assembly callingAssembly, string namespacePrefix)
        {
            var path = Path.GetDirectoryName(callingAssembly.Location);
            DirectoryInfo di = new DirectoryInfo(path);
            var fileNames = di.GetFiles($"{namespacePrefix}*.dll");

            return fileNames.Select(x => Assembly.Load(x.Name)).ToList();
        }

        //NOTE: This didn't work. It left out BookApp.Infrastructure.Book.EventHandlers and others
        internal static List<Assembly> GetProjectAssemblies(this Assembly callingAssembly, string namespacePrefix)
        {
            var returnAssemblies = new List<Assembly>();
            var loadedAssemblies = new HashSet<string>();
            var assembliesToCheck = new Queue<Assembly>();

            assembliesToCheck.Enqueue(callingAssembly);

            bool IsAssemblyInNamespace(AssemblyName assemblyName)
            {
                return !loadedAssemblies.Contains(assemblyName.FullName)
                       && assemblyName.FullName.StartsWith(namespacePrefix);
            }

            while (assembliesToCheck.Any())
            {
                var assemblyToCheck = assembliesToCheck.Dequeue();

                foreach (var reference in assemblyToCheck.GetReferencedAssemblies())
                {
                    if (IsAssemblyInNamespace(reference))
                    {
                        var assembly = Assembly.Load(reference);
                        assembliesToCheck.Enqueue(assembly);
                        loadedAssemblies.Add(reference.FullName);
                        returnAssemblies.Add(assembly);

                        foreach (var referencedAssembly in assembly.GetReferencedAssemblies().Where(IsAssemblyInNamespace))
                        {
                            assembliesToCheck.Enqueue(Assembly.Load(referencedAssembly));
                        }
                    }
                }
            }

            return returnAssemblies;
        }
    }
}