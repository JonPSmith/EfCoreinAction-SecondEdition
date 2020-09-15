// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter12Listings.EventInterfacesEtc;

namespace Test.Chapter12Listings.EventRunnerCode
{
    public class EventSagaRunner : IEventRunner                      
    {
        private readonly IServiceProvider _serviceProvider;          
                                                                     
        public EventSagaRunner(IServiceProvider serviceProvider)     
        {                                                            
            _serviceProvider = serviceProvider;                      
        }                                                            

        public void RunEvents(DbContext context)
        {
            bool shouldRunAgain;                            //#A
            int loopCount = 1;                              //#B
            do                                              //#C
            {
                var allEvents = context. 
                    ChangeTracker.Entries<IEntityEvents>() 
                    .SelectMany(x => x.Entity.GetEventsThenClear()); 

                shouldRunAgain = false;                     //#D
                foreach (var domainEvent in allEvents) 
                {
                    shouldRunAgain = true;                  //#E

                    var domainEventType = domainEvent.GetType(); 
                    var eventHandleType = typeof(IEventHandler<>) 
                        .MakeGenericType(domainEventType); 

                    var eventHandler = 
                        _serviceProvider.GetService(eventHandleType); 
                    if (eventHandler == null) 
                        throw new InvalidOperationException( 
                            $"Could not find an event handler for the event {eventHandleType.Name}");

                    var handlerRunnerType = typeof(EventHandlerRunner<>) 
                        .MakeGenericType(domainEventType); 
                    var handlerRunner = ((EventHandlerRunner) 
                        Activator.CreateInstance( 
                            handlerRunnerType, eventHandler)); 

                    handlerRunner.HandleEvent(domainEvent); 
                }                                               
                if (loopCount++ > 10)                           //#F
                    throw new Exception("Looped to many times");//#F
            } while (shouldRunAgain);                           //#G
        }
    }
    /*********************************************************************
    #A This controls whether the code should loop around again to see if there are any new events
    #B This counts how many times the event runner loops around to check for more events
    #C This do/while code will keep looping while shouldRunAgain is true 
    #D shouldRunAgain is set to false. If there are no events then it will end to do/while
    #E There are events so shouldRunAgain is set to true
    #F This check catches an event handler that triggers circular set of events.   
    #E It stops looping when there are no events to handle
     *********************************************************************/
}