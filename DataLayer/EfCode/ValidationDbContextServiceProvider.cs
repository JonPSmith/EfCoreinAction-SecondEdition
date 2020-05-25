// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode
{
    public class ValidationDbContextServiceProvider : IServiceProvider
    {
        private readonly DbContext _currContext;

        public ValidationDbContextServiceProvider(DbContext currContext)
        {
            _currContext = currContext;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(DbContext))
            {
                return _currContext;
            }
            return null;
        }
    }
}