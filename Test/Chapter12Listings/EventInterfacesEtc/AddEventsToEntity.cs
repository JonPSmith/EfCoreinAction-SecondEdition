// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Test.Chapter12Listings.EventInterfacesEtc
{
    public class AddEventsToEntity : IEntityEvents
    {
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();


        public void AddEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public ICollection<IDomainEvent> GetEventsThenClear()
        {
            var eventsCopy = _domainEvents.ToList();
            _domainEvents.Clear();
            return eventsCopy;
        }
    }
}