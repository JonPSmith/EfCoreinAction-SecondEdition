// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Test.Chapter11Listings.EfCode;

namespace Test.Chapter11Listings.EfClasses
{
    public class Notify2Entity : Notification2Entity
    {
        private int _id;             //#A
        private string _myString;    //#A

        public int Id
        {
            get => _id;
            set => SetWithNotify(value, ref _id); //#B
        }

        public string MyString
        {
            get => _myString;
            set => SetWithNotify(value, ref _myString); //#B
        }
    }
}