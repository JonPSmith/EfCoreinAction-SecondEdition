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
        /********************************************************************
         #A The SaveChangesWithChecking returns a list of ValidationResults. If it is an empty collection then the data was saved. If it has errors then the data wasn't saved
         #B SaveChangesWithChecking is an extension method, which means I can call it in the same way I call SaveChanges
         #C I create a private method to do the validation as I need to apply this in SaveChangesWithChecking and SaveChangesWithCheckingAsync
         #D If there are errors then I return them immediately and don't call SaveChanges
         #E There aren't any errors so I am going to call SaveChanges. 
         #F I return the empty set of errors, which tells the caller that everything is OK
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
        /*************************************************************
        #A I use EF Core's ChangeTracker to get access to all the entity classes it is tracking. NOte: This calls ChangeTracker.DetectChanges, which makes sure all the changes I have made are found
        #B I filter out only those that need to be added to, or update the database
        #C I have created a simple class that implements the IServiceProvider interface, which makes the current DbContext available in the IValidatableObject.Validate method 
        #D The Validator.TryValidateObject is the method which handles all validation checking for me
        #E If there are errors I add them to the list
        #F Finally I return the list of all the errors found
         * *********************************************************/
    }
}