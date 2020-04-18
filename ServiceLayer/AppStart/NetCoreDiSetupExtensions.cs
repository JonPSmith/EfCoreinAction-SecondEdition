// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;

namespace ServiceLayer.AppStart
{
    public static class NetCoreDiSetupExtensions //#A
    {
        public static void RegisterServiceLayerDi //#B
            (this IServiceCollection services)    //#C
        {
            services.RegisterAssemblyPublicNonGenericClasses() //#D
                .AsPublicImplementedInterfaces(); //#E

            //#F

        }
    }
    /****************************************************************************
    #A I create a static class to hold my extension
    #B This class is in the ServiceLayer, so I give the method a name with that Assembly name in it
    #C The NetCore.AutoRegisterDi contains the Microsoft.Extensions.DependencyInjection library, so you can access the IServiceCollection interface
    #D Calling the RegisterAssemblyPublicNonGenericClasses method without a parameter means it scans the calling assembly
    #E This method will register all the public classes with interfaces with a Transient lifetime
    #F This is where you register any other classes/interfaces that can't be registered by the NetCore.AutoRegisterDi, e.g. generic classes
     *************************************************************/
}