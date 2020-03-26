// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Test.Mocks
{
    public class FakeRequestCookieCollection : IRequestCookieCollection
    {
        private List<KeyValuePair<string, string>> _cookieData = new List<KeyValuePair<string, string>>();

        public FakeRequestCookieCollection(string cookieName = null, string cookieContent = null)
        {

            if (cookieName != null)
                _cookieData.Add(new KeyValuePair<string, string>(cookieName, cookieContent));
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _cookieData.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsKey(string key)
        {
            return _cookieData.Any(x => x.Key == key);
        }

        public bool TryGetValue(string key, out string value)
        {
            var found = _cookieData.SingleOrDefault(x => x.Key == key);
            value = found.Key == key ? found.Value : null;
            return found.Key == key;
        }

        public int Count => _cookieData.Count();

        public string this[string key] => _cookieData.SingleOrDefault(x => x.Key == key).Value;

        public ICollection<string> Keys => _cookieData.Select(x => x.Key).ToList();
    }
}