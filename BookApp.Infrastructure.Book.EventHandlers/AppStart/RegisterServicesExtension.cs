// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GenericEventRunner.ForSetup;

namespace BookApp.Infrastructure.Book.EventHandlers.AppStart
{
    public static class RegisterServicesExtension
    {
        public static void RegisterEventHandlers(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterGenericEventRunner();
        }
    }
}