// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace BookApp.Infrastructure.Books.EventHandlers.CheckFixCode
{
    public class CheckFixCacheOptions
    {
        /// <summary>
        /// If false it will check the whole database on startup 
        /// </summary>
        public bool OnlyCheckUpdatesSinceAppStart { get; set; }

        /// <summary>
        /// This will ignore entities whose LastUpdatedUtc is within this offset
        /// The idea is to allow recent update to setting down before we check them
        /// </summary>
        public TimeSpan IgnoreIfWithinOffset { get; set; }

        /// <summary>
        /// This controls how often the CheckFixCacheValuesService is run
        /// </summary>
        public TimeSpan WaitBetweenRuns { get; set; }

        /// <summary>
        /// This controls the the time between checking each updated book 
        /// </summary>
        public TimeSpan WaitBetweenEachCheck { get; set; }

        /// <summary>
        /// If true, then it will fix the error. Otherwise just logs an error
        /// </summary>
        public bool FixBadCacheValues { get; set; }
    }
}