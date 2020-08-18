// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;

namespace BookApp.Infrastructure.Startup
{
    /// <summary>
    /// Inherit if you just want to register the services in a project
    /// </summary>
    public abstract class JustRegisterServices : IRegisterOnStartup
    {
        public void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterAssemblyPublicNonGenericClasses()
                .AsPublicImplementedInterfaces();
        }
    }
}