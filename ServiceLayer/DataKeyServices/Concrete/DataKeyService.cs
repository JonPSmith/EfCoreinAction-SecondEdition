// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using BizLogic.BasketServices;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Http;
using ServiceLayer.CheckoutServices.Concrete;

namespace ServiceLayer.DataKeyServices.Concrete
{
    public class DataKeyService : IDataKeyService
    {
        private readonly IHttpContextAccessor _httpAccessor;

        public DataKeyService(IHttpContextAccessor httpAccessor)
        {
            _httpAccessor = httpAccessor;
        }

        public Guid GetDataKey()
        {
            var httpContext = _httpAccessor?.HttpContext;
            if (httpContext == null)
                return Guid.Empty;

            var cookie = new BasketCookie(httpContext.Request.Cookies);
            var service = new CheckoutCookieService(cookie.GetValue());

            return service.UserId;
        }
    }
}