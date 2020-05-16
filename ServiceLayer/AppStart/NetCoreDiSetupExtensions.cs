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
    #C The NetCore.AutoRegisterDi library understands NET Core DI so you can access the IServiceCollection interface
    #D Calling the RegisterAssemblyPublicNonGenericClasses method without a parameter means it scans the calling assembly
    #E This method will register all the public classes with interfaces with a Transient lifetime
    #F This is for hand-coded registrations that NetCore.AutoRegisterDi can't do, e.g. generic classes
     *************************************************************/
}