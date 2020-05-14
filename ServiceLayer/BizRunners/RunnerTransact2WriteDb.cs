// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BizLogic.GenericInterfaces;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore.Storage;

namespace ServiceLayer.BizRunners
{
    public class RunnerTransact2WriteDb<TIn, TPass, TOut> //#A
        where TPass : class                               //#B
        where TOut : class                                //#B
    {
        private readonly IBizAction<TIn, TPass>           //#C
            _actionPart1;                                 //#C

        private readonly IBizAction<TPass, TOut>          //#C 
            _actionPart2;                                 //#C

        private readonly EfCoreContext _context;

        public RunnerTransact2WriteDb(                    //#E
            EfCoreContext context,                        //#E
            IBizAction<TIn, TPass> actionPart1,           //#E
            IBizAction<TPass, TOut> actionPart2)          //#E 
        {
            _context = context;
            _actionPart1 = actionPart1;
            _actionPart2 = actionPart2;
        }

        public IImmutableList<ValidationResult>           //#D
            Errors { get; private set; }                  //#D

        public bool HasErrors => Errors.Any();            //#D

        public TOut RunAction(TIn dataIn)
        {
            using (var transaction =                      //#F
                _context.Database.BeginTransaction())     //#F
            {
                var passResult = RunPart(                 //#G
                    _actionPart1, dataIn);                //#G
                if (HasErrors) return null;               //#H
                var result = RunPart(                     //#I
                    _actionPart2, passResult);            //#I 
   
                if (!HasErrors)                           //#J
                {                                         //#J
                    transaction.Commit();                 //#J
                }                                         //#J
                return result;                            //#K
            }                                             //#L
        }

        private TPartOut RunPart<TPartIn, TPartOut>(      //#M
            IBizAction<TPartIn, TPartOut> bizPart,        //#M
            TPartIn dataIn)                               //#M
            where TPartOut : class
        {
            var result = bizPart.Action(dataIn);          //#N
            Errors = bizPart.Errors;                      //#N
            if (!HasErrors)                               //#O
            {                                             //#O
                _context.SaveChanges();                   //#O
            }                                             //#O
            return result;                                //#P
        }
    }
    //0123456789|123456789|123456789|123456789|123456789|123456789|123456789|xxxxx!
    /************************************************************************
    #A The three types are: The input, the class passed from Part1 to Part2 and the output
    #B The BizRunner can return null if there are errors, so it has to be a class
    #C This defines the generic BizAction for the two business logic parts
    #D This holds any error information returned by the business logic
    #E The constructor takes both business classes, and the application DbContext 
    #F This start the transaction within a using statement. 
    #G The private method, RunPart, runs the first business part
    #H If there are errors it returns null (the rollback is handled by the dispose)
    #I If the first part of the business logic was successful it runs the second business logic
    #J If there are no errors I commit the transaction to the database
    #K It return the result from the last business logic
    #L If commit is not called before the using end, then RollBack undo all the changes
    #M This private method that handles running each part of the business logic
    #N It runs the business logic and copies the business logic's Errors
    #O If the business logic was successful it call SaveChanges
    #P This return the result from the business logic it ran
     * *****************************************************************/

}