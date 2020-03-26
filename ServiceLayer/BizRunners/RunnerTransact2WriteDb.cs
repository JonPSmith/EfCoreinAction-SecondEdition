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
        where TPass : class //#B
        where TOut : class  //#B
    {
        private readonly IBizAction<TIn, TPass> //#C
            _actionPart1;                       //#C

        private readonly IBizAction<TPass, TOut>//#C 
            _actionPart2;                       //#C

        private readonly EfCoreContext _context;

        public RunnerTransact2WriteDb(          //#E
            EfCoreContext context,              //#E
            IBizAction<TIn, TPass> actionPart1, //#E
            IBizAction<TPass, TOut> actionPart2)//#E 
        {
            _context = context;
            _actionPart1 = actionPart1;
            _actionPart2 = actionPart2;
        }

        public IImmutableList<ValidationResult> //#D
            Errors { get; private set; }        //#D

        public bool HasErrors => Errors.Any();  //#D

        public TOut RunAction(TIn dataIn)
        {
            using (var transaction =                 //#F
                _context.Database.BeginTransaction())//#F
            {
                var passResult = RunPart(  //#G
                    _actionPart1, dataIn); //#G
                if (HasErrors) return null; //#H
                var result = RunPart(         //#I
                    _actionPart2, passResult);//#I 
   
                if (!HasErrors)           //#J
                {                         //#J
                    transaction.Commit(); //#J
                }                         //#J
                return result; //#K
            }
        }

        private TPartOut RunPart<TPartIn, TPartOut>(//#L
            IBizAction<TPartIn, TPartOut> bizPart,  //#L
            TPartIn dataIn)                         //#L
            where TPartOut : class
        {
            var result = bizPart.Action(dataIn);//#M
            Errors = bizPart.Errors;            //#M
            if (!HasErrors)            //#N
            {                          //#N
                _context.SaveChanges();//#N
            }                          //#N
            return result;  //#O
        }
    }
    /************************************************************************
    #A The generic RunnerTransact2WriteDb takes three types: The initial input, the class that is passed from Part1 to Part2 and the final output
    #B Because the BizRunner return null if there is an error then I have to say that the TOut type must be a class
    #C This defines the generic BizAction for the two business logic parts
    #D This holds the error information returned from the last business logic code that ran
    #E The constructor takes the two instances of the business logic, and the application DbContext that the business logic is using
    #F Here I start the transaction on the application's DbContext within a using statement. When it exits the using statement, unless Commit has been called it will RollBack any changes
    #G Here I use a private method, RunPart, to run the first business part
    #H If there are errors I return null (the rollback is handled by the dispose of the transection)
    #I As the first part of the business logic was successful I then run the second part of the business logic
    #J If there are no errors I commit the transaction to the database
    #K I return the result from the last business logic
    #L This is a private method that handles running each part of the business logic
    #M It runs the business logic and copies the business logic's Errors property to the local Errors property
    #N If the business logic was successful I call SaveChanges to apply any add/update/delete commands to the transaction
    #O I return the result from the business logic I ran
     * *****************************************************************/

}