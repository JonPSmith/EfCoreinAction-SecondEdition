// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.
namespace ServiceLayer.Logger
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