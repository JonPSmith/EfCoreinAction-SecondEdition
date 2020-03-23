// =====================================================
// EfCoreExample - Example code to go with book
// Filename: TraceIdentBaseDto.cs
// Date Created: 2016/09/13
// 
// Under the MIT License (MIT)
// 
// Written by Jon P Smith : GitHub JonPSmith, www.thereformedprogrammer.net
// =====================================================
namespace ServiceLayer.Logger
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