// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace BookApp.Infrastructure.LoggingServices
{
    public class TimingStats
    {
        public TimingStats(string overWhat, IReadOnlyList<double> timings, int numPointToConsider)
        {
            OverWhat = overWhat;
            var offset = Math.Max(0, timings.Count - numPointToConsider);
            MinMillisecond = timings.Any() ? Double.MaxValue : 0.0;
            var count = 0;
            for (int i = offset; i < timings.Count; i++)
            {
                AverageMilliseconds += timings[i];
                if (timings[i] > MaxMillisecond)
                    MaxMillisecond = timings[i];
                if (timings[i] < MinMillisecond)
                    MinMillisecond = timings[i];
                count++;
            }

            if (AverageMilliseconds > 0)
                AverageMilliseconds = AverageMilliseconds / count;
        }

        public string OverWhat { get; }
        public double MinMillisecond { get; }
        public double MaxMillisecond { get; }
        public double AverageMilliseconds { get; }
        public int NumPoints { get; }

    }
}