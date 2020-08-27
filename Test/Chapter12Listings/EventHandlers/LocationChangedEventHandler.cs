// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Test.Chapter12Listings.BusinessLogic;
using Test.Chapter12Listings.EfCode;
using Test.Chapter12Listings.EventInterfacesEtc;
using Test.Chapter12Listings.Events;

namespace Test.Chapter12Listings.EventHandlers
{
    public class LocationChangedEventHandler             //#A
        : IEventHandler<LocationChangedEvent>            //#B
    {
        private readonly EventsDbContext _context;       //#C
        private readonly                                 //#C
            IFindSalesTaxService _taxLookupService;      //#C

        public LocationChangedEventHandler(              //#D
            EventsDbContext context,                     //#D
            IFindSalesTaxService taxLookupService)       //#D
        {                                                //#D
            _context = context;                          //#D
            _taxLookupService = taxLookupService;        //#D
        }                                                //#D

        public void HandleEvent
            (LocationChangedEvent domainEvent) //#E
        {
            var salesTaxPercent = _taxLookupService           //#F
                .GetSalesTax(domainEvent.Location.State);     //#F

            foreach (var quote in _context.Quotes.Where(      //#G
                x => x.WhereInstall == domainEvent.Location)) //#G
            {
                quote.SalesTaxPercent = salesTaxPercent;      //#G
            }
        }
    }
    /****************************************************************
    #A This class must be registered as a service via DI
    #B Every event handler must have the interface IEventHandler<T>, where T is the event class type
    #C This specific event handler needs two classes registered with DI
    #D The Event Runner will use DI to get an instance of this class, and will fill in the constructor parameters
    #E This is method from the IEventHandler<T> that the Event Runner will execute
    #F It uses another service to calculate the right sales tax
    #G Then it sets the SaleTax on every Quote that is linked to this location
     ****************************************************************/
}