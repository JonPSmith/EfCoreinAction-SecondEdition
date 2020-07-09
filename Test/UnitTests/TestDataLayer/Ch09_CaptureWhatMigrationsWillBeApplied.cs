// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using DataLayer.Migrations;
using Microsoft.EntityFrameworkCore;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using Xunit.Abstractions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch09_CaptureWhatMigrationsWillBeApplied
    {
        private readonly ITestOutputHelper _output;

        public Ch09_CaptureWhatMigrationsWillBeApplied(ITestOutputHelper output)
        {
            _output = output;
        }

        [RunnableInDebugOnly]
        public void TestDisplayPendingAndAppliedMigrationsOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureDeleted();


                //ATTEMPT
                var willBeApplied = context.Database.GetPendingMigrations();
                _output.WriteLine("PENDING migrations");
                foreach (var filename in willBeApplied)
                {
                    _output.WriteLine($"     {filename}");
                }
                context.Database.Migrate();

                var appliedMigrations = context.Database.GetAppliedMigrations();

                //VERIFY
                _output.WriteLine("APPLIED migrations");
                foreach (var filename in appliedMigrations)
                {
                    _output.WriteLine($"     {filename}");
                }
            }
        }

        [RunnableInDebugOnly]
        public void TestExampleOfFindingAnAppliedMigrationOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureDeleted();

                //ATTEMPT
                context.Database.Migrate(); //#A
                if (CheckIfMigrationWasApplied(context, nameof(InitialMigration))) //#B
                {
                    //... run your C# code for this specific migration //#C
                    _output.WriteLine("Found it!");
                }


                //VERIFY
            }
        }
        
        public static bool CheckIfMigrationWasApplied( //#D
            DbContext context, string className)
        {
            return context.Database.GetAppliedMigrations() //#E
                .Any(x => x.EndsWith(className));          //#E
        }
        /**********************************************************************
        #A You call the migration method to apply any missing migrations to the database
        #B You use the extension method to find if the InitialMigration was added to the database
        #C You place your code here that must run after the InitialMigration has run
        #D Simple extension method to detect a specific migration from the class name
        #E The GetAppliedMigrations method returns a filename of each migration applied to the database
        #F The filenames all end with the class name, so we return true if any filename ends with className 

         **********************************************************/

    }
}