// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BizLogic.GenericInterfaces
{
    public abstract class BizActionErrors //#A
    {
        private readonly List<ValidationResult> _errors //#B
            = new List<ValidationResult>();             //#B

        public IImmutableList<ValidationResult> //#C
            Errors => _errors.ToImmutableList();//#C

        public bool HasErrors => _errors.Any(); //#D

        protected void AddError(string errorMessage, //#E
            params string[] propertyNames)           //#E
        {
            _errors.Add( new ValidationResult  //#F
                (errorMessage, propertyNames));//#F
        }
    }
    /***********************************************************
    #A This is an abstract class that provides error handling for business logic
    #B I hold the list of validation errors privately
    #C But I provide a public, immutable list of errors to the public
    #D I create a simple bool HasErrors to make checking for errors easier
    #E I created this method to allow a simple error message, or an error message with properties linked to it, to be added to the errors list
    #F The validation result has an error message and a, possibly empty, list of properties it is linked to
     * *********************************************************/
}