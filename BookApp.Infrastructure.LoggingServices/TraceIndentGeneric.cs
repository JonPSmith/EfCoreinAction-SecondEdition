// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace BookApp.Infrastructure.LoggingServices
{
    public class TraceIndentGeneric<T> : TraceIdentBaseDto
    {
        public TraceIndentGeneric(string traceIdentifier, T result)
            : base(traceIdentifier)
        {
            Result = result;
        }

        public T Result { get; private set; }
    }
}