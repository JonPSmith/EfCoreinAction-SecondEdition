// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode
{
    public static class DbContextValidationHelper
    {
        //see https://blogs.msdn.microsoft.com/dotnet/2016/09/29/implementing-seeding-custom-conventions-and-interceptors-in-ef-core-1-0/
        //for why I call DetectChanges before ChangeTracker, and why I then turn ChangeTracker.AutoDetectChangesEnabled off/on around SaveChanges

        public static async Task<ImmutableList<ValidationResult>> SaveChangesWithValidationAsync(this DbContext context)
        {
            var result = context.ExecuteValidation();
            if (result.Any()) return result;

            context.ChangeTracker.AutoDetectChangesEnabled = false;
            try
            {
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            finally
            {
                context.ChangeTracker.AutoDetectChangesEnabled = true;
            }   
            return result;
        }

        //see https://blogs.msdn.microsoft.com/dotnet/2016/09/29/implementing-seeding-custom-conventions-and-interceptors-in-ef-core-1-0/
        //for why I call DetectChanges before ChangeTracker, and why I then turn ChangeTracker.AutoDetectChangesEnabled off/on around SaveChanges

        public static ImmutableList<ValidationResult> //#A
            SaveChangesWithValidation(this DbContext context)//#B
        {
            var result = context.ExecuteValidation(); //#C
            if (result.Any()) return result;   //#D

            //I leave out the AutoDetectChangesEnabled on/off from the code shown in chapter 4 as its only a performance issue
            //I'ts a concept that doesn't add anything to chapter 4. However I leave it in the real code as it has a (small) improvement on performance
            context.ChangeTracker.AutoDetectChangesEnabled = false; //LEAVE OUT OF CHAPTER 4 - 
            try
            {
                context.SaveChanges(); //#E
            }
            finally
            {
                context.ChangeTracker.AutoDetectChangesEnabled = true;       //LEAVE OUT OF CHAPTER 4 -      
            }

            return result; //#F
        }
        //0123456789|123456789|123456789|123456789|123456789|123456789|123456789|xxxxx!
        /********************************************************************
         #A The SaveChangesWithValidation returns a list of ValidationResults.
         #B SaveChangesWithChecking is an extension method. 
         #C The ExecuteValidation is used in SaveChangesWithChecking/SaveChangesWithCheckingAsync
         #D If there are errors then I return them immediately and don't call SaveChanges
         #E There aren't any errors so I am going to call SaveChanges. 
         #F This return the empty set of errors to signify there are no errors
         * *****************************************************************/

        private static ImmutableList<ValidationResult>
            ExecuteValidation(this DbContext context)
        {
            var result = new List<ValidationResult>();
            foreach (var entry in 
                context.ChangeTracker.Entries() //#A
                    .Where(e =>
                       (e.State == EntityState.Added) ||   //#B
                       (e.State == EntityState.Modified))) //#B
            {
                var entity = entry.Entity;
                var valProvider = new 
                    ValidationDbContextServiceProvider(context);//#C
                var valContext = new 
                    ValidationContext(entity, valProvider, null);
                var entityErrors = new List<ValidationResult>();
                if (!Validator.TryValidateObject(           //#D
                    entity, valContext, entityErrors, true))//#D
                {
                    result.AddRange(entityErrors); //#E
                }
            }
            return result.ToImmutableList(); //#F
        }
        //0123456789|123456789|123456789|123456789|123456789|123456789|123456789|xxxxx!
        /*************************************************************
        #A This uses EF Core's ChangeTracker to get access to all the entity classes it is tracking
        #B This filters the entities that will be added or updated in the database
        #C This implements the IServiceProvider interface and passes the DbContext to the Validate method
        #D The Validator.TryValidateObject is the method validates each class
        #E Any errors are added to the list
        #F Finally it returns the list of all the errors found. Empty if no errors
         * *********************************************************/
    }
}