// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using DataLayer.Interfaces;

namespace Test.Chapter11Listings
{
    public class CompanySoftCascade : ICascadeSoftDelete
    {
        public int CompanySoftCascadeId { get; set; }

        public string CompanyName { get; set; }

        public HashSet<QuoteSoftCascade> Quotes { get; set; }
        public byte SoftDeleteLevel { get; set; }
    }
}