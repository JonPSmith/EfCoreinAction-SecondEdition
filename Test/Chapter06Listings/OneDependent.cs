// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Test.Chapter06Listings
{
    public class OneDependent
    {
        public int Id { get; set; }

        public int OnePrincipalId { get; set; }

        public OnePrincipal Link { get; set; }
    }
}