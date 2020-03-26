// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Http;

namespace ServiceLayer.Utils
{
    public abstract class CookieTemplate
    {

        private readonly IRequestCookieCollection _cookiesIn;
        private readonly IResponseCookies _cookiesOut;  
        private readonly string _cookieName;

        protected CookieTemplate(string cookieName, IRequestCookieCollection cookiesIn, IResponseCookies cookiesOut = null)
        {
            if (cookiesIn == null)
                throw new ArgumentNullException(nameof(cookiesIn));

            _cookiesIn = cookiesIn;
            _cookiesOut = cookiesOut;
            _cookieName = cookieName;
        }

        //----------------------------------------------------
        //parts that can be overrriden to change how it works

        /// <summary>
        /// Override to to set the expiration time of the cookie in days.
        /// If not overriden then returns 0 which means cookie Expiration is not set and it becomes a session cookie
        /// </summary>
        protected virtual int ExpiresInThisManyDays { get { return 0; } }

        //-----------------------------------------------------
        //Now the public interfaces

        public void AddOrUpdateCookie(string value)
        {
            if (_cookiesOut == null)
                throw new NullReferenceException("You must supply a IResponseCookies value if you want to use this command.");

            var options = new CookieOptions();
            if (ExpiresInThisManyDays > 0)
                //set/update expires if ExpiresInThisManyDays has been overrriden with postive num
                options.Expires = DateTime.Now.AddDays(ExpiresInThisManyDays);
            _cookiesOut.Append(_cookieName, value, options);
        }

        public bool Exists()
        {
            return _cookiesIn[_cookieName] != null;
        }

        public string GetValue()
        {
            var cookie = _cookiesIn[_cookieName];
            return string.IsNullOrEmpty(cookie) ? null : cookie;
        }

        public void DeleteCookie()
        {
            if (_cookiesOut == null)
                throw new NullReferenceException("You must supply a IResponseCookies value if you want to use this command.");

            if (!Exists()) return;
            var options = new CookieOptions {Expires = DateTime.Now.AddYears(-1)};
            _cookiesOut.Append(_cookieName, "", options);
        }


    }
}