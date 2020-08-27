// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using Test.Chapter12Listings.EfCode;

namespace Test.Chapter12Listings.BusinessLogic
{
    public class FindSalesTaxService : IFindSalesTaxService
    {
        private readonly EventsDbContext _context;

        public FindSalesTaxService(EventsDbContext context)
        {
            _context = context;
        }

        public double GetSalesTax(string state)
        {
            if (state == null)
                return _context.SalesTaxes.Select(x => x.SalesTaxPercent).Max();

            var foundStateTax = _context.SalesTaxes.SingleOrDefault(x => x.State == state)?.SalesTaxPercent;
            if (foundStateTax == null)
                throw new InvalidOperationException($"No tax information for state {state}");
            return (double)foundStateTax;
        }
    }
}