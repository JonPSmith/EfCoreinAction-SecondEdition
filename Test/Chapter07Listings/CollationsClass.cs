// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Test.Chapter07Listings
{
    public class CollationsClass
    {
        public int Id { get; set; }

        public string NormalString { get; set; }

        public string CaseSensitiveString { get; set; }

        public string CaseSensitiveStringWithIndex { get; set; }

        private CollationsClass() {}
        public CollationsClass(string setAll)
        {
            NormalString = setAll;
            CaseSensitiveString = setAll;
            CaseSensitiveStringWithIndex = setAll;
        }
    }
}