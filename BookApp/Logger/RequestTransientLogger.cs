// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServiceLayer.Logger;

namespace EfCoreInAction.Logger
{
    /// <summary>
    /// This logger only logs for the current request, i.e. it overwrites the log when a new request starts
    /// </summary>
    public class RequestTransientLogger : ILoggerProvider
    {
        private readonly Func<IHttpContextAccessor> _httpAccessor;

        public static LogLevel LogThisAndAbove { get; set; } = LogLevel.Information;

        public RequestTransientLogger(Func<IHttpContextAccessor> httpAccessor)
        {
            _httpAccessor = httpAccessor;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new MyLogger(_httpAccessor);
        }


        public void Dispose()
        {
        }

        private class MyLogger : ILogger
        {
            private readonly Func<IHttpContextAccessor> _httpAccessor;

            public MyLogger(Func<IHttpContextAccessor> httpAccessor)
            {
                _httpAccessor = httpAccessor;
                
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return logLevel >= LogThisAndAbove;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                Func<TState, Exception, string> formatter)
            {
                var currHttpContext = _httpAccessor().HttpContext;
                if (currHttpContext == null)
                    return; //we ignore any logs that happen outside a HttpRequest

                HttpRequestLog.AddLog(currHttpContext.TraceIdentifier,
                    logLevel, eventId, formatter(state, exception));
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
        }
    }
}