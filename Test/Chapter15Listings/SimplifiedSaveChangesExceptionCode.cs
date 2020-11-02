// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;

namespace Test.Chapter15Listings
{
    public class SimplifiedSaveChangesExceptionCode
    {
        private IStatusGeneric<int> //#A
            CallSaveChangesWithExceptionHandler
            (DbContext context, 
                Func<int> callBaseSaveChanges)  //#B
        {
            var status = new StatusGenericHandler<int>(); //#C

            do //#D
            {
                try
                {
                    int numUpdated = callBaseSaveChanges(); //#E
                    status.SetResult(numUpdated);  //#F
                    break;                         //#F
                }
                catch (Exception e) //#G
                {
                    IStatusGeneric handlerStatus = null; //#H
                    if (handlerStatus == null)   //#I
                        throw;                   //#I
                    status.CombineStatuses(handlerStatus); //#J
                }

            } while (status.IsValid); //#K

            return status;  //#L
        }
        /******************************************************
        #A The returned value is a status, with int return from SaveChanges
        #B The base SaveChanges is provided to be called
        #C This is the status that will be returned
        #D The call to the SaveChanges is done within a do/while
        #E You call the base SaveChanges
        #F If no exception, then set the status result and break out of the do/while
        #G The catch caches any exceptions that SaveChanges throws
        #H Your exception handler is called here, and it returns null or a status
        #I If the exception handler returns null it rethrows the original exception
        #J Otherwise any errors from your exception handler are added to the main status
        #K If the exception handler was successful it loops back to try calling SaveChanges again
        #L Finally it returns the status
         *******************************************************/
    }
}