// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace BookApp.Infrastructure.LoggingServices
{
    public class TraceIdentBaseDto
    {
        public string TraceIdentifier { get; private set; }

        public int NumLogs { get; private set; }

        public TraceIdentBaseDto(string traceIdentifier)
        {
            TraceIdentifier = traceIdentifier;
            NumLogs = HttpRequestLog.GetHttpRequestLog(traceIdentifier).RequestLogs.Count;
        }
    }
}