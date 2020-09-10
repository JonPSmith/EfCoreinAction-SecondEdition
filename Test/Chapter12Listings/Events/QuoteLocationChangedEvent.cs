// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Test.Chapter12Listings.DomainEventEfClasses;
using Test.Chapter12Listings.EventInterfacesEtc;

namespace Test.Chapter12Listings.Events
{
    public class QuoteLocationChangedEvent : IDomainEvent
    {
        public QuoteLocationChangedEvent(Quote quoteToUpdate, Location newLocation)
        {
            QuoteToUpdate = quoteToUpdate;
            NewLocation = newLocation;
        }

        public Location NewLocation { get; }
        public Quote QuoteToUpdate { get; }
    }
}