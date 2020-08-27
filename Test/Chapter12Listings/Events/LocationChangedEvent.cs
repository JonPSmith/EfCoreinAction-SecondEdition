// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Test.Chapter12Listings.EfClasses;
using Test.Chapter12Listings.EventInterfacesEtc;

namespace Test.Chapter12Listings.Events
{
    public class LocationChangedEvent : IDomainEvent
    {
        public LocationChangedEvent(Location location, string newState)
        {
            NewState = newState;
        }

        public Location Location { get; }
        public string NewState { get; }
    }
}