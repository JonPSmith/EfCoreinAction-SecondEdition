// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MyFirstEfCoreApp
{
    public class MyLoggerProvider : ILoggerProvider
    {
        private readonly List<string> _logs;

        public MyLoggerProvider(List<string> logs)
        {
            _logs = logs;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new MyLogger(_logs);
        }

        public void Dispose()
        { }

        private class MyLogger : ILogger
        {
            private readonly List<string> _logs;

            public MyLogger(List<string> logs)
            {
                _logs = logs;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return logLevel >= LogLevel.Information;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                _logs.Add(formatter(state, exception));
                //Console.WriteLine(formatter(state, exception));
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
        }
    }
}