// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Test.Mocks
{
    public class FakeResponseCookies : IResponseCookies
    {
        public List<KeyValuePair<string, string>> Responses { get; private set; } =
            new List<KeyValuePair<string, string>>();

        public void Append(string key, string value)
        {
            Responses.Add(new KeyValuePair<string, string>(key, value));
        }

        public void Append(string key, string value, CookieOptions options)
        {
            Responses.Add(new KeyValuePair<string, string>(key, $"{value}; {options.ToString()}"));
        }

        public void Delete(string key)
        {
            var found = Responses.SingleOrDefault(x => x.Key == key);
            if (found.Key == key)
                Responses.Remove(found);
        }

        public void Delete(string key, CookieOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}