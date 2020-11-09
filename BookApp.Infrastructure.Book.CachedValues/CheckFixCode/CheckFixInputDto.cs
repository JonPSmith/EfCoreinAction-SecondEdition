// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace BookApp.Infrastructure.Books.CachedValues.CheckFixCode
{
    public class CheckFixInputDto
    {
        public TimeSpan LookingBack { get; set; }
        public bool FixBadCacheValues { get; set; }
    }
}