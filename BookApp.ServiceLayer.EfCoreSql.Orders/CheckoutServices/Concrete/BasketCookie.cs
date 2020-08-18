// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Http;

namespace BookApp.ServiceLayer.EfCoreSql.Orders.CheckoutServices.Concrete
{
    public class BasketCookie : CookieTemplate
    {
        public const string BasketCookieName = "EfCoreInAction2-basket";

        public BasketCookie(IRequestCookieCollection cookiesIn, IResponseCookies cookiesOut = null) 
            : base(BasketCookieName, cookiesIn, cookiesOut)
        {
        }

        protected override int ExpiresInThisManyDays => 200;    //Make this last, as it holds the user id for checking orders
    }
}