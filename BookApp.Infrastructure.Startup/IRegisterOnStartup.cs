// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookApp.Infrastructure.Startup
{
    public interface IRegisterOnStartup
    {
        void RegisterServices(IServiceCollection services, IConfiguration configuration);
    }
}