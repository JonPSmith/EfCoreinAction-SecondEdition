// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using DataLayer.EfClasses;

namespace ServiceLayer.AdminServices
{
    public interface IChangePriceOfferService
    {
        Book OrgBook { get; }

        PriceOffer GetOriginal(int id);
        ValidationResult AddRemovePriceOffer(PriceOffer promotion);
    }
}