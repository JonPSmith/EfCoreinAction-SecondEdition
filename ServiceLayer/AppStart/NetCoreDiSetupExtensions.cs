// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;

namespace ServiceLayer.AppStart
{
    public static class NetCoreDiSetupExtensions
    {
        public static void RegisterServiceLayerDi(this IServiceCollection services)
        {
            services.RegisterAssemblyPublicNonGenericClasses()
                .Where(c => c.Name.EndsWith("Service"))
                .AsPublicImplementedInterfaces();

            //register any services that can't be handled by RegisterAssemblyPublicNonGenericClasses

        }
    }
}