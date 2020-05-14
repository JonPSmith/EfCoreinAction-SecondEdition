// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfClasses
{
    public class LineItem : IValidatableObject //#A
    {
        public int LineItemId { get; set; }

        [Range(1,5, ErrorMessage =                      //#B
            "This order is over the limit of 5 books.")] //#B
        public byte LineNum { get; set; }

        public short NumBooks { get; set; }

        /// <summary>
        /// This holds a copy of the book price. We do this in case the price of the book changes,
        /// e.g. if the price was discounted in the future the order is still correct.
        /// </summary>
        public decimal BookPrice { get; set; }

        // relationships

        public int OrderId { get; set; }
        public int BookId { get; set; }

        public Book ChosenBook { get; set; }

        IEnumerable<ValidationResult> IValidatableObject.Validate //#C
            (ValidationContext validationContext)                 //#C
        {
            var currContext = 
                validationContext.GetService(typeof(DbContext));//#D

            if (ChosenBook.Price < 0)                      //#E
                yield return new ValidationResult(         //#E
$"Sorry, the book '{ChosenBook.Title}' is not for sale."); //#E

            if (NumBooks > 100)
                yield return new ValidationResult(//#F
"If you want to order a 100 or more books"+       //#F
" please phone us on 01234-5678-90",              //#F
                    new[] { nameof(NumBooks) });  //#F
        }
    }
    //0123456789|123456789|123456789|123456789|123456789|123456789|123456789|xxxxx!
    /**********************************************************
    #A The IValidatableObject interface adds a IValidatableObject.Validate method
    #B This will add an error message if the LineNum property is not in range
    #C This is the method that the IValidatableObject interface requires me to create
    #D This allows access the current DbContext if needed to get more information. 
    #D Here I use the ChosenBook link to look at the date the book was published.
    #E This moves the Price check out of the business logic
    #F This tests a property so I can return that property with the error.
     * *******************************************************/
}