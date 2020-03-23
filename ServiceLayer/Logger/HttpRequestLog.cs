// =====================================================
// EfCoreExample - Example code to go with book
// Filename: HttpRequestLog.cs
// Date Created: 2016/09/11
// 
// Under the MIT License (MIT)
// 
// Written by Jon P Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("test")]

namespace ServiceLayer.Logger
{
    /// <summary>
    /// This class handles the storing/retrieval of logs for each Http request, as defined by 
    /// ASP.NET Core's TraceIdentifier. 
    /// It uses a static ConcurrentDictionary to hold the logs. 
    /// NOTE: THIS WILL NOT WORK WITH SCALE OUT, i.e. it will not work if multiple instances of the web app are running
    /// </summary>
    public class HttpRequestLog
    {
        private const int MaxKeepLogMinutes = 10;

        private static readonly ConcurrentDictionary<string, HttpRequestLog> AllHttpRequestLogs = new ConcurrentDictionary<string, HttpRequestLog>();

        private readonly List<LogParts> _requestLogs;

        public string TraceIdentifier { get; }

        public DateTime LastAccessed { get; private set; }

        public ImmutableList<LogParts> RequestLogs => _requestLogs.ToImmutableList();

        public override string ToString()
        {
            return $"At time: {LastAccessed:s}, Logs : {string.Join("/n", _requestLogs.Select(x => x.ToString()))}";
        }

        private HttpRequestLog(string traceIdentifier)
        {
            TraceIdentifier = traceIdentifier;
            LastAccessed = DateTime.UtcNow;
            _requestLogs = new List<LogParts>();

            //now clear old request logs
            ClearOldLogs(MaxKeepLogMinutes);
        }

        public static void AddLog(string traceIdentifier, LogLevel logLevel, EventId eventId, string eventString)
        {
            var thisSessionLog = AllHttpRequestLogs.GetOrAdd(traceIdentifier,
                x => new HttpRequestLog(traceIdentifier));

            thisSessionLog._requestLogs.Add(new LogParts(logLevel, eventId, eventString));
            thisSessionLog.LastAccessed = DateTime.UtcNow;
        }

        /// <summary>
        /// This returns the HttpRequestLog for the given traceIdentifier
        /// </summary>
        /// <param name="traceIdentifier"></param>
        /// <returns>found HttpRequestLog. returns null of not found (log might be old)</returns>
        public static HttpRequestLog GetHttpRequestLog(string traceIdentifier)
        {
            HttpRequestLog result;
            if (AllHttpRequestLogs.TryGetValue(traceIdentifier, out result)) return result;

            //No log so make up one to say what has happened.
            result = new HttpRequestLog(traceIdentifier);
            var oldest = AllHttpRequestLogs.Values.OrderBy(x => x.LastAccessed).FirstOrDefault();
            result._requestLogs.Add(new LogParts(LogLevel.Warning, new EventId(1, "EfCoreInAction"), 
                $"Could not find the log you asked for. I have {AllHttpRequestLogs.Keys.Count} logs" +
                (oldest == null ? "." : $" the oldest is {oldest.LastAccessed:s}")));

            return result;
        }

        //-----------------------------------
        //private methods

        /// <summary>
        /// Made internal so Unit Tests can get at it (not ideal, but needed)
        /// </summary>
        /// <param name="maxKeepLogMinutes"></param>
        internal static void ClearOldLogs(int maxKeepLogMinutes)
        {
            var logsToRemove =
                AllHttpRequestLogs.Values.OrderBy(x => x.LastAccessed).Where(
                    x => DateTime.UtcNow.Subtract(x.LastAccessed).TotalMinutes > maxKeepLogMinutes);

            RemoveLogs(logsToRemove);
        }

        private static void RemoveLogs(IEnumerable<HttpRequestLog> logsToRemove)
        {
            foreach (var logToRemove in logsToRemove)
            {
                HttpRequestLog value;
                AllHttpRequestLogs.TryRemove(logToRemove.TraceIdentifier, out value);
            }
        }
    }
}