// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using BookApp.BizLogic.Orders.BasketServices;
using BookApp.Persistence.EfCoreSql.Orders;
using BookApp.ServiceLayer.EfCoreSql.Orders.CheckoutServices.Concrete;
using Microsoft.AspNetCore.Http;

namespace BookApp.ServiceLayer.EfCoreSql.Orders.UserIdServices
{
    public class UserIdService : IUserIdService
    {
        private readonly IHttpContextAccessor _httpAccessor;            

        public UserIdService(IHttpContextAccessor httpAccessor)         
        {                                                               
            _httpAccessor = httpAccessor;                               
        }                                                               

        public Guid GetUserId()
        {
            var httpContext = _httpAccessor.HttpContext;                
            if (httpContext == null)                                    
                return Guid.Empty;                                      

            var cookie = new BasketCookie(httpContext.Request.Cookies); 
            if (!cookie.Exists())                                       
                return Guid.Empty;                                      

            var service = new CheckoutCookieService(cookie.GetValue()); 
            return service.UserId;                                      
        }
    }
}