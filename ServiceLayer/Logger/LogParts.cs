// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServiceLayer.Logger
{
    public class LogParts
    {
        private const string EfCoreEventIdStartWith = "Microsoft.EntityFrameworkCore";

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

        public bool IsDb => EventId.Name?.StartsWith(EfCoreEventIdStartWith) ?? false;

        public override string ToString()
        {
            return $"{LogLevel}: {EventString}";
        }
    }
}