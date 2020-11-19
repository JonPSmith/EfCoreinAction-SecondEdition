// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;

namespace BookApp.Infrastructure.LoggingServices
{
    public class TimingThisUrl
    {
        private const int LastNToAverage = 5;

        public TimingThisUrl(string url, List<double> timings, DateTime lastAccessed, int lastN)
        {
            Url = url;
            LastAccessed = lastAccessed;
            LastN = lastN;
            Timings = timings;

            AllTimings = new TimingStats("All", timings, int.MaxValue);
            LastNTimings = new TimingStats($"Last {LastNToAverage}", timings, LastNToAverage);
        }

        public string Url { get; }
        public DateTime LastAccessed { get; }
        public int LastN { get; }
        public List<double> Timings { get; }

        public TimingStats AllTimings { get; }
        public TimingStats LastNTimings { get; }
    }
}