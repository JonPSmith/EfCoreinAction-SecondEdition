// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Test.Chapter12Listings.EventInterfacesEtc;

namespace Test.Chapter12Listings.EventRunnerCode
{
    internal abstract class EventHandlerRunnerAsync               //#A        
    {
        public abstract Task HandleEventAsync                   //#A
            (IDomainEvent domainEvent);                      //#A
    }

    internal class EventHandlerRunnerAsync<T> : EventHandlerRunnerAsync //#B
        where T : IDomainEvent
    {
        private readonly IEventHandlerAsync<T> _handler;           //#C

        public EventHandlerRunnerAsync(IEventHandlerAsync<T> handler)   //#C
        {                                                     //#C
            _handler = handler;                               //#C
        }                                                     //#C

        public override Task HandleEventAsync                      //#D
            (IDomainEvent domainEvent)                        //#D
        {                                                     //#D
            return _handler.HandleEventAsync((T)domainEvent);             //#D
        }                                                     //#D
    }
    /****************************************************
    #A By defining a non-generic method you can run the generic event handler
    #B This uses the EventHandlerRunner<T> to define the type of the EventHandlerRunner
    #C The EventHandlerRunner class is created with an instance of the event handler to run
    #D Now to the method the abstract class defined.
     ***********************************************/
}