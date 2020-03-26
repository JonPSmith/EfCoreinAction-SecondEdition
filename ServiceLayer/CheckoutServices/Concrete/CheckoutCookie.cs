// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Http;
using ServiceLayer.Utils;

namespace ServiceLayer.CheckoutServices.Concrete
{
    public class CheckoutCookie : CookieTemplate
    {
        public const string CheckoutCookieName = "EfCoreInAction-Checkout";

        public CheckoutCookie(IRequestCookieCollection cookiesIn, IResponseCookies cookiesOut = null) 
            : base(CheckoutCookieName, cookiesIn, cookiesOut)
        {
        }

        protected override int ExpiresInThisManyDays => 200;    //Make this last, as it holds the user id for checking orders
    }
}