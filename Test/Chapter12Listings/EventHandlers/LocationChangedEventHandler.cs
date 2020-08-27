// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Test.Chapter12Listings.BusinessLogic;
using Test.Chapter12Listings.EfCode;
using Test.Chapter12Listings.EventInterfacesEtc;
using Test.Chapter12Listings.Events;

namespace Test.Chapter12Listings.EventHandlers
{
    public class LocationChangedEventHandler : IEventHandler<LocationChangedEvent>
    {
        private readonly EventsDbContext _context;
        private readonly IFindSalesTaxService _taxLookupService;

        public LocationChangedEventHandler(EventsDbContext context, IFindSalesTaxService taxLookupService)
        {
            _context = context;
            _taxLookupService = taxLookupService;
        }

        public void HandleEvent(LocationChangedEvent domainEvent)
        {
            var salesTaxPercent = _taxLookupService.GetSalesTax(domainEvent.Location.State);
            //For each Quote that is linked to this location needs changing
            foreach (var quote in _context.Quotes.Where(x => x.WhereInstall == domainEvent.Location ))
            {
                quote.SalesTaxPercent = salesTaxPercent;
            }
        }
    }
}