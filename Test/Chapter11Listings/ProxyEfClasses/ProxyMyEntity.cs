// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Test.Chapter11Listings.ProxyEfClasses
{
    public class ProxyMyEntity
    {
        public virtual int Id { get; set; }

        public virtual string MyString { get; set; }

        public virtual ProxyOptional ProxyOptional { get; set; }
    }
}