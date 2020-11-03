// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using BookApp.Infrastructure.Books.EventHandlers.CheckFixCode;
using Microsoft.Extensions.Options;

namespace Test.TestHelpers
{
    public class MockCheckFixIOptions : IOptions<CheckFixCacheOptions>
    {
        public MockCheckFixIOptions(bool fixBadCacheValues = true)
        {
            Value = new CheckFixCacheOptions
            {
                OnlyCheckUpdatesSinceAppStart = false, //Check all
                WaitBetweenRuns = new TimeSpan(0, 0, 0, 0), //No wait
                WaitBetweenEachCheck = new TimeSpan(0, 0, 0, 0), //No wait
                IgnoreIfWithinOffset = new TimeSpan(0, 0, 0, 0), //No wait
                FixBadCacheValues = fixBadCacheValues
            };
        }

        public CheckFixCacheOptions Value { get; set; }

    }
}