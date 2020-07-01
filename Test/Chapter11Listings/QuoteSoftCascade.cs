// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.Interfaces;

namespace Test.Chapter11Listings
{
    public class QuoteSoftCascade : ICascadeSoftDelete
    {
        public int QuoteSoftCascadeId { get; set; }

        public int CompanySoftCascade { get; set; }

        public CompanySoftCascade BelongsTo { get; set; }

        public byte SoftDeleteLevel { get; set; }
    }
}