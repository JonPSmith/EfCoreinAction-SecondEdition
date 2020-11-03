// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace BookApp.Infrastructure.Books.EventHandlers.CheckFixCode
{
    public class CheckFixCacheOptions
    {
        public DateTime? IgnoreBeforeDateUtc { get; set; }
        public TimeSpan IgnoreAfterOffset { get; set; }

        public TimeSpan WaitBetweenRuns { get; set; }
        public TimeSpan WaitBetweenEachCheck { get; set; }
        public bool FixBadCacheValues { get; set; }
    }
}