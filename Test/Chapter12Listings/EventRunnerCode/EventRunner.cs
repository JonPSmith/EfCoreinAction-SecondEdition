// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter12Listings.EventInterfacesEtc;
using Test.Chapter12Listings.Events;

namespace Test.Chapter12Listings.EventRunnerCode
{
    public class EventRunner : IEventRunner
    {
        private readonly IServiceProvider _serviceProvider;

        public EventRunner(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void RunEvents(DbContext context)
        {
            var allEvents  = context.ChangeTracker.Entries<IEntityEvents>()
                .SelectMany(x => x.Entity.GetEventsThenClear());

            foreach (var domainEvent in allEvents)
            {
                var domainEventType = domainEvent.GetType();
                var handleType = typeof(IEventHandler<>).MakeGenericType(domainEventType);
                var eventHandler = _serviceProvider.GetService(handleType);
                if (eventHandler == null)
                    throw new InvalidOperationException(
                        $"Could not find an event handler for the event {handleType.Name}");

                var handlerRunner =
                    ((EventHandlerRunner) Activator.CreateInstance(typeof(EventHandlerRunner<>).MakeGenericType(domainEventType), eventHandler));
                handlerRunner.HandleEvent(domainEvent);
            }
        }
    }
}