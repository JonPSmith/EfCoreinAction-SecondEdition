// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Test.Chapter11Listings.ProxyEfClasses
{
    public class ProxyMyEntity
    {
        public virtual int Id { get; set; }                       //#A
        public virtual string MyString { get; set; }              //#A
        public virtual ProxyOptional ProxyOptional { get; set; }  //#A

        public virtual ObservableCollection<ProxyMany>            //#A
            Many { get; set; }                                    //#B
            = new ObservableCollection<ProxyMany>();              //#B
    }
    /****************************************************************
    #A All properties must be virtual
    #B For navigational collection properties you need to use an Observable collection type
     ********************************************************/
}