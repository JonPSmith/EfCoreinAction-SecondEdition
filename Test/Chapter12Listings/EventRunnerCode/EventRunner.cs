// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter12Listings.EventInterfacesEtc;

namespace Test.Chapter12Listings.EventRunnerCode
{
    public class EventRunner : IEventRunner                          //#A
    {
        private readonly IServiceProvider _serviceProvider;          //#B
                                                                     //#B
        public EventRunner(IServiceProvider serviceProvider)         //#B
        {                                                            //#B
            _serviceProvider = serviceProvider;                      //#B
        }                                                            //#B

        public void RunEvents(DbContext context)
        {
            var allEvents  = context.                                //#C
                ChangeTracker.Entries<IEntityEvents>()               //#C
                .SelectMany(x => x.Entity.GetEventsThenClear());     //#C

            foreach (var domainEvent in allEvents)                   //#D
            {
                var domainEventType = domainEvent.GetType();         //#E
                var eventHandleType = typeof(IEventHandler<>)        //#E
                    .MakeGenericType(domainEventType);               //#E

                var eventHandler =                                   //#F
                    _serviceProvider.GetService(eventHandleType);    //#F
                if (eventHandler == null)                            //#F
                    throw new InvalidOperationException(             //#F
                        $"Could not find an event handler for the event {eventHandleType.Name}");

                var handlerRunnerType = typeof(EventHandlerRunner<>) //#G
                    .MakeGenericType(domainEventType);               //#G
                var handlerRunner = ((EventHandlerRunner)            //#G
                    Activator.CreateInstance(                        //#G
                        handlerRunnerType, eventHandler));           //#G

                handlerRunner.HandleEvent(domainEvent);              //#H
            }
        }
    }
    /*********************************************************************
    #A The EventRunner needs an interface to that you can register it with the DI
    #B The event runner needs the ServiceProvider to get an instance of the event handlers
    #C This reads in all the events. It also clears the entity events to stops duplicate events
    #D This loops through each event found
    #E This gets the interface type of the matching event handler
    #F This uses the DI provider to create an instance of the event handler - error if not found
    #G This creates the EventHandlerRunner that you need to run the event handler
    #H Then you use the EventHandlerRunner to run the event handler
     *********************************************************************/
}