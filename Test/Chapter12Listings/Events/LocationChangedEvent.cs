// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Test.Chapter12Listings.EfClasses;
using Test.Chapter12Listings.EventInterfacesEtc;

namespace Test.Chapter12Listings.Events
{
    public class LocationChangedEvent : IDomainEvent   //#A
    {
        public LocationChangedEvent(Location location)
        {
            Location = location;
        }

        public Location Location { get; }               //#B
    }
    /*****************************************************
    #A The event class must inherit the IDomainEvent
    #B The event handler needs Location to be able to do the Quote updates
     ***************************************************/
}