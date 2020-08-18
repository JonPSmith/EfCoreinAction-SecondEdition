// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using BookApp.Infrastructure.Startup;
using GenericEventRunner.ForSetup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookApp.Infrastructure.Book.EventHandlers.AppStart
{
    public class RegisterEventHandlers : IRegisterOnStartup
    {
        public void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterGenericEventRunner();
        }
    }
}