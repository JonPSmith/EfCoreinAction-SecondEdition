// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using BizLogic.GenericInterfaces;
using DataLayer.EfCode;

namespace ServiceLayer.BizRunners
{
    public class RunnerWriteDb<TIn, TOut>
    {
        private readonly IBizAction<TIn, TOut> _actionClass;
        private readonly EfCoreContext _context;

        public RunnerWriteDb(                 //#B
            IBizAction<TIn, TOut> actionClass,//#B
            EfCoreContext context)            //#B
        {
            _context = context;
            _actionClass = actionClass;
        }

        public IImmutableList<ValidationResult>         //#A
            Errors => _actionClass.Errors;              //#A

        public bool HasErrors => _actionClass.HasErrors;//#A

        public TOut RunAction(TIn dataIn) //#C
        {                                               
            var result = _actionClass.Action(dataIn); //#D
            if (!HasErrors)            //#E
                _context.SaveChanges();//#E
            return result; //#F
        }
    }
    //0123456789|123456789|123456789|123456789|123456789|123456789|123456789|xxxxx!
    /*********************************************************
    #A The error information from the business logic is passed back to the user of the BizRunner
    #B This BizRunner handles business logic that conforms to the IBizAction<TIn, TOut> interface
    #C I call RunAction in my Service Layer, or in my Presentation Layer if the data comes back in the right form
    #D It runs the business logic I gave it
    #E If there aren't any errors it calls SaveChanges to execute any add, update or delete methods
    #F Finally it returns the result that the business logic returned
     * ******************************************************/
}