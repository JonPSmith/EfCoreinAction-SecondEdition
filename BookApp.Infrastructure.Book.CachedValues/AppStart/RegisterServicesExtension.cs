// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using BookApp.Infrastructure.Books.CachedValues.CheckFixCode;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookApp.Infrastructure.Books.CachedValues.AppStart
{
    public static class RegisterServicesExtension
    {
        public static void RegisterCheckFixCacheValuesService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ICheckFixCacheValuesService, CheckFixCacheValuesService>();
        }
    }
}