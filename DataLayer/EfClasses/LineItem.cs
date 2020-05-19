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
    /**********************************************************
    #A By applying the IValidatableObject interface then the validation will call the method the interface defines
    #B This is one of the validation DataAnnotations. The validator will show my error message if the LineNum property is not in range
    #C This is the method that the IValidatableObject interface requires me to create
    #D I can access the current DbContext that this database access is using. In this case I don't use it, but you could use it to get better error feedback information for the user
    #D Here I use the ChosenBook link to look at the date the book was published. I can also format my own error message, which is helpful
    #E This moves the Price check out of the business logic
    #F This tests a property in this class so I can return that property with the error.
     * *******************************************************/
}