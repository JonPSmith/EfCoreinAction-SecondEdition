// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Test.Chapter12Listings.EventInterfacesEtc;

namespace Test.Chapter12Listings.EventRunnerCode
{
    internal abstract class EventHandlerRunner               //#A        
    {
        public abstract void HandleEvent                     //#A
            (IDomainEvent domainEvent);                      //#A
    }

    internal class EventHandlerRunner<T> : EventHandlerRunner //#B
        where T : IDomainEvent
    {
        private readonly IEventHandler<T> _handler;           //#C

        public EventHandlerRunner(IEventHandler<T> handler)   //#C
        {                                                     //#C
            _handler = handler;                               //#C
        }                                                     //#C

        public override void HandleEvent                      //#D
            (IDomainEvent domainEvent)                        //#D
        {                                                     //#D
            _handler.HandleEvent((T)domainEvent);             //#D
        }                                                     //#D
    }
    /****************************************************
    #A By defining a non-generic class+method you can cast the created generic instance to an non-generic version
    #B This uses the EventHandlerRunner<T> to define the type of the EventHandlerRunner
    #C The EventHandlerRunner class is created with an instance of the event handler to run
    #D Now to the method the abstract class defined.
     ***********************************************/
}