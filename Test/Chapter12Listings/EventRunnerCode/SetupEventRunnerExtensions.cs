// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Test.Chapter12Listings.EventInterfacesEtc;

namespace Test.Chapter12Listings.EventRunnerCode
{
    public static class SetupEventRunnerExtensions
    {
        public static void RegisterEventRunnerAndHandlers(
            this IServiceCollection services,                     //#A
            params Assembly[] assembliesToScan)                   //#B
        {
            services.AddTransient<IEventRunner, EventRunner>();      //#C

            foreach (var assembly in assembliesToScan)            //#D
            {                                                     //#D
                services.RegisterEventHandlers(assembly);         //#D
            }                                                     //#D
        }

        private static void RegisterEventHandlers(                //#E
            this IServiceCollection services,                     //#E
            Assembly assembly)                                    //#E
        {                                                        
            var allGenericClasses = assembly.GetExportedTypes()   //#F
                .Where(y => y.IsClass && !y.IsAbstract            //#F
                   && !y.IsGenericType && !y.IsNested);           //#F
            var classesWithIHandle =                              //#G
                from classType in allGenericClasses               //#G
                let interfaceType = classType.GetInterfaces()     //#G
                    .SingleOrDefault(y =>                         //#G
                        y.IsGenericType &&                        //#G
                        y.GetGenericTypeDefinition() ==           //#G
                            typeof(IEventHandler<>))              //#G
                where interfaceType != null                       //#G
                select (interfaceType, classType);                //#G

            foreach (var tuple in classesWithIHandle)             //#H
            {                                                     //#H
                services.AddTransient(                            //#H
                    tuple.interfaceType, tuple.classType);        //#H
            }                                                     //#H
        }
        /****************************************************************
        #A The method needs the NET Core's service collection to add to
        #B You provide one of more assemblies to scan
        #C You register the Event Runner
        #D This calls a method to find and register event handler in an assembly
        #E This will find and register all the classes that have the IEventHandler<T> interface
        #F This finds all the classes that could be an event handler in the assembly
        #G This finds all the classes that have the IEventHandler<T> interface, plus the interface type
        #H Then it registers each class with their interface
         ************************************************************/

    }
}