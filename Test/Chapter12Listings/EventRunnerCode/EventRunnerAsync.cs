// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Test.Chapter12Listings.EventInterfacesEtc;

namespace Test.Chapter12Listings.EventRunnerCode
{
    public class EventRunnerAsync : IEventRunnerAsync                
    {
        private readonly IServiceProvider _serviceProvider;          
                                                                     
        public EventRunnerAsync(IServiceProvider serviceProvider)    
        {                                                            
            _serviceProvider = serviceProvider;                      
        }                                                            

        public async Task RunEventsAsync(DbContext context)          //#A
        {
            var allEvents  = context.                                
                ChangeTracker.Entries<IEntityEvents>()               
                .SelectMany(x => x.Entity.GetEventsThenClear());     

            foreach (var domainEvent in allEvents)                   
            {
                var domainEventType = domainEvent.GetType();         
                var eventHandleType = typeof(IEventHandlerAsync<>)   //#B
                    .MakeGenericType(domainEventType);               

                var eventHandler =                                   
                    _serviceProvider.GetService(eventHandleType);    
                if (eventHandler == null)                            
                    throw new InvalidOperationException(             
                        $"Could not find an event handler for the event {eventHandleType.Name}");

                var handlerRunnerType = 
                    typeof(EventHandlerRunnerAsync<>)                 //#C
                    .MakeGenericType(domainEventType);               
                var handlerRunner = ((EventHandlerRunnerAsync)        //#D   
                    Activator.CreateInstance(                        
                        handlerRunnerType, eventHandler));           

                await handlerRunner.HandleEventAsync(domainEvent);    //#E
            }
        }
        /*********************************************************************
        #A The RunEvent because an async method RunEventAsync
        #B The code is now looking for a handle with an async type
        #C It needs a async EventHandlerRunner to run the event handler
        #D And it is cast to a async method
        #E This allows the code to run the async event handler
         *********************************************************************/
    }
}