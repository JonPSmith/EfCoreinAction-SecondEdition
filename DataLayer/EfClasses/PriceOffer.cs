// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace DataLayer.EfClasses
{
    public class PriceOffer
    {
        public const int PromotionalTextLength = 200;

        public int PriceOfferId { get; set; }
        public decimal NewPrice { get; set; }
        [Required]
        [MaxLength(PromotionalTextLength)]
        public string PromotionalText { get; set; }

        //-----------------------------------------------
        //Relationships

        public int BookId { get; set; }
    }
}