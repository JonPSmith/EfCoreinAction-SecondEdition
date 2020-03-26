// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Test.Mocks
{
    public class MockHttpCookieAccess
    {
        private readonly FakeResponseCookies _fakeResponse = new FakeResponseCookies();

        public MockHttpCookieAccess(string cookieName = null, string cookieContent = null)
        {
            CookiesIn = new FakeRequestCookieCollection(cookieName, cookieContent);
            CookiesOut = _fakeResponse;
        }

        public IRequestCookieCollection CookiesIn { get; private set; }

        public IResponseCookies CookiesOut { get; private set; }

        public List<KeyValuePair<string, string>> ResponseCookieValues => _fakeResponse.Responses;
    }
}