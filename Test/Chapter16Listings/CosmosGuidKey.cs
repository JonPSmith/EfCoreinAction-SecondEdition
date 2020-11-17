// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace Test.Chapter16Listings
{
    public class CosmosGuidKey
    {
        public Guid Id { get; set; }
        public DateTime MyDateTime { get; set; }
        public TimeSpan MyTimSpan { get; set; }

        public string NullableProp { get; set; }
    }
}