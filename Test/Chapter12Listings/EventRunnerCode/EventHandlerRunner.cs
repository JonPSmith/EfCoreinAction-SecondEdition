// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Test.Chapter12Listings.EventInterfacesEtc;
using Test.Chapter12Listings.Events;

namespace Test.Chapter12Listings.EventRunnerCode
{
    internal abstract class EventHandlerRunner
    {
        public abstract void HandleEvent(IDomainEvent domainEvent);
    }

    internal class EventHandlerRunner<T> : EventHandlerRunner
        where T : IDomainEvent
    {
        private readonly IEventHandler<T> _handler;

        public EventHandlerRunner(IEventHandler<T> handler)
        {
            _handler = handler;
        }

        public override void HandleEvent(IDomainEvent domainEvent)
        {
            _handler.HandleEvent((T)domainEvent);
        }
    }
}