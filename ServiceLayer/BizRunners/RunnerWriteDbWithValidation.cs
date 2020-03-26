// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BizLogic.GenericInterfaces;
using DataLayer.EfCode;

namespace ServiceLayer.BizRunners
{
    public class RunnerWriteDbWithValidation<TIn, TOut>
    {
        private readonly IBizAction<TIn, TOut> _actionClass;
        private readonly EfCoreContext _context;

        public RunnerWriteDbWithValidation(    //#B
            IBizAction<TIn, TOut> actionClass, //#B
            EfCoreContext context)             //#B
        {
            _context = context;
            _actionClass = actionClass;
        }

        public IImmutableList<ValidationResult>//#A
            Errors { get; private set; }       //#A

        public bool HasErrors => Errors.Any(); //#A

        public TOut RunAction(TIn dataIn) //#C
        {                                               
            var result = _actionClass.Action(dataIn);  //#D
            Errors = _actionClass.Errors;  //#E
            if (!HasErrors) //#F
            {
                Errors =                              //#G
                    _context.SaveChangesWithValidation()//#G
                        .ToImmutableList();           //#G
            }
            return result; //#H
        }
    }
    /*********************************************************
    #A In this version I need my own Errors and HasErrors properties, as errors can come from two sources
    #B This BizRunner handles business logic that conforms to the IBizAction<TIn, TOut> interface
    #C I call RunAction in my Service Layer, or in my Presentation Layer if the data comes back in the right form
    #D It runs the business logic I gave it
    #E Now I have to assign any errors from the business logic to my local errors list
    #F If there aren't any errors I call SaveChangesWithChecking to execute any add, update or delete methods
    #G I extract the error message part of the ValidationResults and assign the list to my Errors 
    #F Finally it returns the result that the business logic returned
     * ******************************************************/
}