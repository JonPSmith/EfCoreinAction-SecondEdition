// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Test.Chapter12Listings.EventInterfacesEtc
{
    public class AddEventsToEntity : IEntityEvents      //#A
    {
        private readonly List<IDomainEvent>             //#B
            _domainEvents = new List<IDomainEvent>();   //#B

        public void AddEvent(IDomainEvent domainEvent)  //#C
        {                                               //#C
            _domainEvents.Add(domainEvent);             //#C
        }                                               //#C

        public ICollection<IDomainEvent>                //#D
            GetEventsThenClear()                        //#D
        {                                               //#D
            var eventsCopy = _domainEvents.ToList();    //#D
            _domainEvents.Clear();                      //#D
            return eventsCopy;                          //#D
        }                                               //#D
    }
    /************************************************************
    #A The IEntityEvents defines the GetEventsThenClear method for the Event Runner
    #B The list of IDomainEvent events are stored in a field 
    #C The AddEvent is used to add new events to the _domainEvents list
    #D This method is called by the Event Runner to get the events and then clear the list
     ***********************************************************/
}