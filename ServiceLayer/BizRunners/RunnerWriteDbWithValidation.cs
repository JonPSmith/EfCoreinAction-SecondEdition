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
    //0123456789|123456789|123456789|123456789|123456789|123456789|123456789|xxxxx!
    /*********************************************************
    #A Ths version needs its own Errors/HasErrors properties, as errors come from two sources
    #B This handles business logic that conforms to the IBizAction<TIn, TOut> interface
    #C This method is called to execute the business logic and handle any errors
    #D It runs the business logic I gave it
    #E Any errors from the business logic are assigned to the local errors list
    #F If there aren't any errors it calls SaveChangesWithChecking
    #G Any validation errors are assigned the the Errors list
    #H Finally it returns the result that the business logic returned
     * ******************************************************/
}