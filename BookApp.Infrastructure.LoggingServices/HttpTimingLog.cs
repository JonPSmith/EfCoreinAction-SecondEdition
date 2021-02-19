// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace BookApp.Infrastructure.LoggingServices
{
    public class HttpTimingLog
    {
        private static HttpTimingLog _lastTimingLog;

        private static readonly string[] UrlsToIgnore = new string[]
        {
            "http://localhost:59382/favicon.ico",
            "http://localhost:59382/Admin/GetTimingLogs",
            "http://localhost:59382/Logger/GetLog",
            "http://localhost:59382/DefaultSql/GetFilterSearchContent",
            "http://localhost:59382/CosmosEf/GetFilterSearchContent",
            "http://localhost:59382/CosmosDirect/GetFilterSearchContent",
            "http://localhost:59382/DapperSql/GetFilterSearchContent",
            "http://localhost:59382/DapperNoCountSql/GetFilterSearchContent",
            "http://localhost:59382/CachedSql/GetFilterSearchContent",
            "http://localhost:59382/CachedNoCountSql/GetFilterSearchContent",
            "http://localhost:59382/UdfsSql/GetFilterSearchContent",
            "http://localhost:59382/lib/"
        };

        public HttpTimingLog(string loggedUrl)
        {
            LoggedUrl = loggedUrl;
            LastAccessed = DateTime.UtcNow;
            Timings = new List<double>();
        }

        public string LoggedUrl { get; }
        public DateTime LastAccessed { get; private set; }
        public List<double> Timings { get; }


        public static void AddLog(string url, string eventString)
        {
            if (UrlsToIgnore.Any(url.StartsWith))
                return;

            if (_lastTimingLog?.LoggedUrl != url)
                _lastTimingLog = new HttpTimingLog(url);

            _lastTimingLog.Timings.Add(DecodeTiming(eventString));
            _lastTimingLog.LastAccessed = DateTime.UtcNow;
        }

        public static TimingThisUrl GetTimingStats(int lastN)
        {
            return new TimingThisUrl(_lastTimingLog.LoggedUrl, _lastTimingLog.Timings, _lastTimingLog.LastAccessed, lastN);
        }

        //-----------------------------------
        //private methods

        private static double DecodeTiming(string eventString)
        {
            var lastSpace = eventString.LastIndexOf(' ');
            var numString = eventString.Substring(lastSpace, eventString.Length - lastSpace - 2);
            return double.TryParse(numString, out double result)
                ? result
                : -1000;
        }
    }
}