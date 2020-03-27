// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Test.Mocks;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestSupportCode
{
    public class Ch04_MockHttpCookieAccess
    {
        [Fact]
        public void CreateEmptyCookiesAccess()
        {
            //SETUP

            //ATTEMPT
            var mockCookieRequests = new MockHttpCookieAccess();

            //VERIFY
            mockCookieRequests.CookiesIn.ShouldNotBeNull();
            mockCookieRequests.CookiesIn.Count.ShouldEqual(0);
            mockCookieRequests.CookiesOut.ShouldNotBeNull();
        }

        [Fact]
        public void CreateWithExistingInCookie()
        {
            //SETUP

            //ATTEMPT
            var mockCookieRequests = new MockHttpCookieAccess("Test", "Content");

            //VERIFY
            mockCookieRequests.CookiesIn.ShouldNotBeNull();
            mockCookieRequests.CookiesIn.Count.ShouldEqual(1);
            mockCookieRequests.CookiesIn["Test"].ShouldEqual("Content");
        }

        [Fact]
        public void AddOutCookie()
        {
            //SETUP
            var mockCookieRequests = new MockHttpCookieAccess();

            //ATTEMPT
            mockCookieRequests.CookiesOut.Append("Test", "Content");

            //VERIFY
            mockCookieRequests.ResponseCookieValues.Count.ShouldEqual(1);
            mockCookieRequests.ResponseCookieValues.First().ShouldEqual(
                new KeyValuePair<string, string>("Test", "Content"));
        }
    }
}