// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BookApp.Infrastructure.LoggingServices
{
    public class LogParts
    {
        private const string EfCoreEventIdStartWith = "Microsoft.EntityFrameworkCore";
        public const string DapperEventName = "EfCoreInAction.Dapper";
        public const string CosmosEventName = "EfCoreInAction.CosmosDirect";

        public LogParts(LogLevel logLevel, EventId eventId, string eventString)
        {
            LogLevel = logLevel;
            EventId = eventId;
            EventString = eventString;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public LogLevel LogLevel { get; private set; }

        public EventId EventId { get; private set; }

        public string EventString { get; private set; }

        public bool IsDb
        {
            get
            {
                var name = EventId.Name;
                return name != null && (name.StartsWith(EfCoreEventIdStartWith)
                                        || name.StartsWith(DapperEventName)
                                        || name.StartsWith(CosmosEventName));
            }
        }

        public override string ToString()
        {
            return $"{LogLevel}: {EventString}";
        }
    }
}