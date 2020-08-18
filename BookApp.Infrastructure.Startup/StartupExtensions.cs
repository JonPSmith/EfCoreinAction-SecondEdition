// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Test")]

namespace BookApp.Infrastructure.Startup
{
    public static class StartupExtensions
    {
        public static IServiceCollection RegisterServicesInAllProjects(
            this IServiceCollection services, IConfiguration configuration, Assembly startAssembly = null)
        {
            startAssembly ??= Assembly.GetCallingAssembly();
            var namespacePrefix = startAssembly.GetName().Name;
            namespacePrefix = namespacePrefix.Substring(0, namespacePrefix.IndexOf('.'));
            var allProjectAssembles = GetProjectAssemblies(startAssembly, namespacePrefix);

            return services;
        }


        internal static List<Assembly> GetProjectAssemblies(Assembly callingAssembly, string namespacePrefix)
        {
            var returnAssemblies = new List<Assembly>();
            var loadedAssemblies = new HashSet<string>();
            var assembliesToCheck = new Queue<Assembly>();

            assembliesToCheck.Enqueue(callingAssembly);

            while (assembliesToCheck.Any())
            {
                var assemblyToCheck = assembliesToCheck.Dequeue();

                foreach (var reference in assemblyToCheck.GetReferencedAssemblies())
                {
                    if (!loadedAssemblies.Contains(reference.FullName) 
                    && reference.FullName.StartsWith(namespacePrefix))
                    {
                        var assembly = Assembly.Load(reference);
                        assembliesToCheck.Enqueue(assembly);
                        loadedAssemblies.Add(reference.FullName);
                        returnAssemblies.Add(assembly);
                    }
                }
            }

            return returnAssemblies;
        }
    }
}