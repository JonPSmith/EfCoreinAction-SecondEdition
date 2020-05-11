// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using BizLogic.BasketServices;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Http;
using ServiceLayer.CheckoutServices.Concrete;

namespace ServiceLayer.DataKeyServices.Concrete
{
    public class UserIdService : IUserIdService
    {
        private readonly IHttpContextAccessor _httpAccessor;            //#A

        public UserIdService(IHttpContextAccessor httpAccessor)         //#A
        {                                                               //#A
            _httpAccessor = httpAccessor;                               //#A
        }                                                               //#A

        public Guid GetUserId()
        {
            var httpContext = _httpAccessor.HttpContext;                //#B
            if (httpContext == null)                                    //#B
                return Guid.Empty;                                      //#B

            var cookie = new BasketCookie(httpContext.Request.Cookies); //#C
            if (!cookie.Exists())                                       //#C
                return Guid.Empty;                                      //#C

            var service = new CheckoutCookieService(cookie.GetValue()); //#D
            return service.UserId;                                      //#D
        }
    }
    /******************************************************
    #A The IHttpContextAccessor is a way to access the current HTTP context. To use this you need to register this in Statup class using the command 'services.AddHttpContextAccessor()'
    #B There are cases where the HTTPContext could be null, say in a background task. In this case you provide an empty GUID
    #C This uses existing services to look for the basket cookie. If there is no cookie then it returns an empty GUID
    #D If there is a basket cookie then you create the CheckoutCookieService which extracts the UserId. This UserId it returned
     ******************************************************/
}